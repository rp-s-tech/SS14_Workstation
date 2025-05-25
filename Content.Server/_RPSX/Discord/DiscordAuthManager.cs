using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Server.RPSX.Discord
{
    public interface IDiscordAuthManager
    {
        void Initialize();
        Task<bool> IsVerifiedAsync(NetUserId userId);
        void RefreshVerification(NetUserId userId);
    }

    public sealed class DiscordAuthManager : IDiscordAuthManager
    {
        private readonly Dictionary<NetUserId, bool> _verificationCache = new();

        private readonly HttpClient _httpClient = new();
        private ISawmill _sawmill = default!;
        private string _apiUrl = string.Empty;

        [Dependency] private readonly IConfigurationManager _cfg = default!;

        public void Initialize()
        {
            _sawmill = Logger.GetSawmill("discord_auth");
            _cfg.OnValueChanged(RPSXCCVars.DiscordAuthServer, s => _apiUrl = s, true);
        }

        public async Task<bool> IsVerifiedAsync(NetUserId userId)
        {
            if (_verificationCache[userId] == true)
                return true;

            if (string.IsNullOrEmpty(_apiUrl))
            {
                _verificationCache[userId] = false;
                return false;
            }

            var url = $"{_apiUrl}/discord_auth/{userId.ToString()}";
            var response = await _httpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _sawmill.Info($"Received Discord verification: NOT FOUND");
                    _verificationCache[userId] = false;
                    return false;
                case HttpStatusCode.OK:
                    var isVerified = await response.Content.ReadFromJsonAsync<bool>();
                    _sawmill.Info($"Received Discord verification: {isVerified}");
                    _verificationCache[userId] = isVerified;
                    return isVerified;
            }

            var errorText = await response.Content.ReadAsStringAsync();
            _sawmill.Error(
                "Failed to get player verification info from API: [{StatusCode}] {Response}",
                response.StatusCode,
                errorText);

            _verificationCache[userId] = false;
            return false;
        }

        public void RefreshVerification(NetUserId userId)
        {
            _verificationCache.Remove(userId);
        }
    }
}
