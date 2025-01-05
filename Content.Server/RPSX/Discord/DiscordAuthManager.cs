using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Utility;
using Serilog;

namespace Content.Server.RPSX.Discord
{
    public sealed class DiscordAuthInfo
    {
        public string? Uid { get; set; }
        public string? Discord { get; set; }
        public string? Verify { get; set; }
    }

    public interface IDiscordAuthManager
    {
        void Initialize();

        Task<DiscordAuthInfo?> LoadDiscordAuthInfo(NetUserId userId);

        bool TryGetDiscordId(NetUserId userId, [NotNullWhen(true)] out string? discordId);

        void CacheDiscordId(NetUserId userId, string discordId);
    }

    public sealed class DiscordAuthManager : IDiscordAuthManager
    {
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        private readonly HttpClient _httpClient = new();
        private ISawmill _sawmill = default!;
        private string _apiUrl = string.Empty;
        private readonly Dictionary<NetUserId, string> _cachedDiscordIds = new();

        public void Initialize()
        {
            _sawmill = Logger.GetSawmill("discord-auth");
            _cfg.OnValueChanged(RPSXCCVars.DiscordAuthServer, s => _apiUrl = s, true);
        }

        public async Task<DiscordAuthInfo?> LoadDiscordAuthInfo(NetUserId userId)
        {
            if (string.IsNullOrEmpty(_apiUrl))
            {
                _sawmill.Error("DiscordAuthManager: discord.auth_server (RPSXCCVars.DiscordAuthServer) не настроен!");
                return null;
            }

            var url = $"{_apiUrl.TrimEnd('/')}/{userId}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var discordInfo = await response.Content.ReadFromJsonAsync<DiscordAuthInfo>();
                    if (discordInfo != null)
                    {
                        if (!string.IsNullOrEmpty(discordInfo.Discord))
                        {
                            CacheDiscordId(userId, discordInfo.Discord);
                        }

                        _sawmill.Info($"DiscordAuthManager: Получены данные от API для пользователя {userId}: " +
                                    $"uid={discordInfo.Uid}, discord={discordInfo.Discord}, verify={discordInfo.Verify}");
                        return discordInfo;
                    }

                    _sawmill.Warning($"DiscordAuthManager: Пустые данные от API для {userId}");
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _sawmill.Info($"DiscordAuthManager: Пользователь {userId} не найден в API.");
                    return null;
                }

                _sawmill.Error($"DiscordAuthManager: Ошибка от API [{response.StatusCode}]: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                _sawmill.Error($"DiscordAuthManager: Не удалось подключиться к {url}. Ошибка: {ex.Message}");
            }

            return null;
        }

        public bool TryGetDiscordId(NetUserId userId, [NotNullWhen(true)] out string? discordId)
        {
            return _cachedDiscordIds.TryGetValue(userId, out discordId);
        }

        public void CacheDiscordId(NetUserId userId, string discordId)
        {
            _cachedDiscordIds[userId] = discordId;
            _sawmill.Info($"DiscordAuthManager: Кэш Discord ID обновлён для пользователя {userId}: discord={discordId}");
        }
    }
}
