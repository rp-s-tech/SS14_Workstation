using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Content.Shared.RPSX.Patron;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Serilog;

namespace Content.Server.RPSX.Sponsors;

public sealed class SponsorsManager : ISponsorsManager
{
    [Dependency] private readonly IServerNetManager _netMgr = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IServerDbManager _serverDbManager = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;

    private readonly HttpClient _httpClient = new();

    private ISawmill _sawmill = default!;
    private string _apiUrl = string.Empty;

    private readonly Dictionary<NetUserId, string> _cachedSponsors = new();
    private readonly Dictionary<NetUserId, string> _cachedAdditionalSponsors = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(RPSXCCVars.SponsorsApiUrl, s => _apiUrl = s, true);

        _netMgr.RegisterNetMessage<MsgSponsorInfo>();
        _netMgr.RegisterNetMessage<MsgAdditionalSponsorInfo>();

        _netMgr.Connecting += OnConnecting;
        _netMgr.Connected += OnConnected;
        _netMgr.Disconnect += OnDisconnect;
    }

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsorTier)
    {
        sponsorTier = null;
        return _cachedSponsors.TryGetValue(userId, out var tierId) && _prototype.TryIndex(tierId, out sponsorTier);
    }

    public bool TryGetAdditionalSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsorTier)
    {
        sponsorTier = null;
        return _cachedAdditionalSponsors.TryGetValue(userId, out var tierId) && _prototype.TryIndex(tierId, out sponsorTier);
    }

    public bool IsJobAvailable(NetUserId userId, JobPrototype job)
    {
        var isWhiteListEnabled = _cfg.GetCVar(CCVars.GameRoleWhitelist);
        if (isWhiteListEnabled && job is { SponsorIgnore: false })
            return false;

        return TryGetSponsorTier(userId, out var sponsorTier) && sponsorTier.RoleTimeByPass;
    }

    public bool IsUserHasRoleTimeByPass(NetUserId userId)
    {
        return TryGetSponsorTier(userId, out var sponsorTier) && sponsorTier.RoleTimeByPass;
    }

    private async Task OnConnecting(NetConnectingArgs e)
    {
        var info = await LoadSponsorInfo(e.UserId);
        var additionalInfo = await _serverDbManager.GetAdditionalSponsorTier(e.UserId);
        if (info?.TierId == null)
        {
            _cachedSponsors.Remove(e.UserId); // Remove from cache if sponsor expired
        }
        else
        {
            DebugTools.Assert(!_cachedSponsors.ContainsKey(e.UserId), "Cached data was found on client connect");
            _cachedSponsors[e.UserId] = info.TierId;
        }
        if (additionalInfo == null)
        {
            _cachedAdditionalSponsors.Remove(e.UserId);
        }
        else
        {
            DebugTools.Assert(!_cachedAdditionalSponsors.ContainsKey(e.UserId), "Cached data was found on client connect");
            _cachedAdditionalSponsors[e.UserId] = additionalInfo;
        }
    }

    private async void OnConnected(object? sender, NetChannelArgs e)
    {
        var tierId = _cachedSponsors.TryGetValue(e.Channel.UserId, out var sponsor) ? sponsor : null;
        var msg = new MsgSponsorInfo { TierId = tierId };
        _netMgr.ServerSendMessage(msg, e.Channel);

        var tier = _cachedAdditionalSponsors.TryGetValue(e.Channel.UserId, out var additionalSponsor) ? additionalSponsor : null;
        var amsg = new MsgAdditionalSponsorInfo { TierId = tier };
        _netMgr.ServerSendMessage(amsg, e.Channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs e)
    {
        _cachedSponsors.Remove(e.Channel.UserId);
        _cachedAdditionalSponsors.Remove(e.Channel.UserId);
    }

    private async Task<SponsorInfo?> LoadSponsorInfo(NetUserId userId)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return null;

        var url = $"{_apiUrl}/sponsors/{userId.ToString()}";
        var response = await _httpClient.GetAsync(url);
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                _sawmill.Info($"Received SponsorInfo: NULL");
                return null;
            case HttpStatusCode.OK:
                var sponsorInfo = await response.Content.ReadFromJsonAsync<SponsorInfo>();
                _sawmill.Info($"Received SponsorInfo: TierId = {sponsorInfo?.TierId}");
                return sponsorInfo;
        }

        var errorText = await response.Content.ReadAsStringAsync();
        _sawmill.Error(
            "Failed to get player sponsor OOC color from API: [{StatusCode}] {Response}",
            response.StatusCode,
            errorText);

        return null;
    }

    public async void AddSponsor(NetUserId userId, SponsorTier tier, int days)
    {
        await _serverDbManager.ChangeAdditionalSponsorTier(userId, tier, days);
        _sawmill.Info("Sponsor Added {userId} {tier} for {days}", userId, tier.ID, days);
    }

    public async void RemoveSponsor(NetUserId userId)
    {
        await _serverDbManager.ChangeAdditionalSponsorTier(userId);
        _sawmill.Info("Sponsor Removed {userId}", userId);
    }
}
