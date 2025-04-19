using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Content.Server.Database;
using Robust.Shared.Network;

namespace Content.Server.RPSX.Discord
{
    public interface IDiscordAuthManager
    {
        Task<bool> IsVerifiedAsync(NetUserId userId);
        void RefreshVerification(NetUserId userId);
    }

    public sealed class DiscordAuthManager : IDiscordAuthManager
    {
        private readonly IServerDbManager _dbManager;
        private readonly Dictionary<NetUserId, bool> _verificationCache = new();

        public DiscordAuthManager(IServerDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<bool> IsVerifiedAsync(NetUserId userId)
        {
            if (!_verificationCache.TryGetValue(userId, out var isVerified))
            {
                isVerified = await _dbManager.IsDiscordVerifiedAsync(userId);
                _verificationCache[userId] = isVerified;
            }

            return isVerified;
        }

        public void RefreshVerification(NetUserId userId)
        {
            _verificationCache.Remove(userId);
        }
    }
}
