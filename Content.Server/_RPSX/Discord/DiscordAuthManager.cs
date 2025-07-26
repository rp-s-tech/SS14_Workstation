using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using System.Collections.Concurrent;

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
        private readonly Dictionary<NetUserId, (bool Value, DateTime? Expire)> _verificationCache = new();
        private readonly ConcurrentDictionary<NetUserId, Task<bool>> _activeRequests = new();

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
            if (_verificationCache.TryGetValue(userId, out var cached))
            {
                if (cached.Value && cached.Expire == null)
                    return true;
                if (!cached.Value && cached.Expire > DateTime.UtcNow)
                    return false;
            }

            var task = _activeRequests.GetOrAdd(userId, _ => InternalVerifyAsync(userId));
            try
            {
                return await task;
            }
            finally
            {
                _activeRequests.TryRemove(new KeyValuePair<NetUserId, Task<bool>>(userId, task));
            }
        }

        private async Task<bool> InternalVerifyAsync(NetUserId userId)
        {
            if (string.IsNullOrEmpty(_apiUrl))
            {
                return false;
            }

            var url = $"{_apiUrl}/discord_auth/{userId.ToString()}";
            var response = await _httpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _sawmill.Info($"Received Discord verification: NOT FOUND");
                    lock (_verificationCache)
                        _verificationCache[userId] = (false, DateTime.UtcNow.AddSeconds(1));
                    return false;
                case HttpStatusCode.OK:
                    var isVerified = await response.Content.ReadFromJsonAsync<bool>();
                    _sawmill.Info($"Received Discord verification: {isVerified}");
                    lock (_verificationCache)
                    {
                        if (isVerified)
                            _verificationCache[userId] = (true, null);
                        else
                            _verificationCache[userId] = (false, DateTime.UtcNow.AddSeconds(1));
                    }
                    return isVerified;
            }

            var errorText = await response.Content.ReadAsStringAsync();
            _sawmill.Error(
                "Failed to get player verification info from API: [{StatusCode}] {Response}",
                response.StatusCode,
                errorText);
            lock (_verificationCache)
                _verificationCache[userId] = (false, DateTime.UtcNow.AddSeconds(1));
            return false;
        }

        public void RefreshVerification(NetUserId userId)
        {
            lock (_verificationCache)
                _verificationCache.Remove(userId);
        }
    }
}
