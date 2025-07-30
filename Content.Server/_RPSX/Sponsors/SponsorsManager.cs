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

    private readonly HttpClient _httpClient = new();

    private ISawmill _sawmill = default!;
    private string _apiUrl = string.Empty;

    private readonly Dictionary<NetUserId, AllSponsorInfo> _cachedSponsors = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(RPSXCCVars.SponsorsApiUrl, s => _apiUrl = s, true);

        _netMgr.RegisterNetMessage<MsgSponsorInfo>();

        _netMgr.Connecting += OnConnecting;
        _netMgr.Connected += OnConnected;
        _netMgr.Disconnect += OnDisconnect;
    }

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out AllSponsorInfo? sponsorInfo)
    {
        sponsorInfo = null;
        return _cachedSponsors.TryGetValue(userId, out sponsorInfo);
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
        if (info?.TierId == null && additionalInfo == null)
        {
            _cachedSponsors.Remove(e.UserId); // Remove from cache if sponsor expired
            return;
        }
        DebugTools.Assert(!_cachedSponsors.ContainsKey(e.UserId), "Cached data was found on client connect");
        SponsorTier? tier;
        var newInfo = new AllSponsorInfo();
        if (info?.TierId != null)
        {
            tier = _prototype.Index<SponsorTier>(info.TierId);
            newInfo.AvailableItems = Math.Min(2, newInfo.AvailableItems + tier.AvailableItems);
            newInfo.RoleTimeByPass |= tier.RoleTimeByPass;
            newInfo.HavePriorityJoin |= tier.HavePriorityJoin;

            newInfo.AllowedMarkings = tier.AllowedMarkings;
            newInfo.AllowedLoadouts = tier.AllowedLoadouts;
            newInfo.AllowedSpecies = tier.AllowedSpecies;
            newInfo.PetCategories = tier.PetCategories;
            newInfo.Ghosts = tier.Ghosts;
        }

        if (additionalInfo != null)
        {
            newInfo.AvailableItems = Math.Min(2, newInfo.AvailableItems + additionalInfo.AvailableItems);
            newInfo.RoleTimeByPass |= additionalInfo.RoleTimeByPass;
            newInfo.HavePriorityJoin |= additionalInfo.HavePriorityJoin;

            AddUniqueItems(newInfo.AllowedMarkings, additionalInfo.AllowedMarkings);
            AddUniqueItems(newInfo.AllowedLoadouts, additionalInfo.AllowedLoadouts);
            AddUniqueItems(newInfo.AllowedSpecies, additionalInfo.AllowedSpecies);
            AddUniqueItems(newInfo.PetCategories, additionalInfo.PetCategories);
            AddUniqueItems(newInfo.Ghosts, additionalInfo.Ghosts);
        }
        _cachedSponsors[e.UserId] = newInfo;
    }

    private async void OnConnected(object? sender, NetChannelArgs e)
    {
        var tierId = _cachedSponsors.TryGetValue(e.Channel.UserId, out var sponsor) ? sponsor : null;
        var msg = new MsgSponsorInfo { TierId = tierId };
        _netMgr.ServerSendMessage(msg, e.Channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs e)
    {
        _cachedSponsors.Remove(e.Channel.UserId);
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

    public void AddSponsor(NetUserId userId, SponsorTier tier, int days)
    {
        AddSponsorPrivate(userId, tier, days);
    }

    private async void AddSponsorPrivate(NetUserId userId, SponsorTier tier, int days)
    {
        await _serverDbManager.ChangeAdditionalSponsorTier(userId, tier, days);
        _sawmill.Info("Sponsor added: {userId} {tier} for {days}", userId, tier.ID, days);
    }

    public void RemoveSponsor(NetUserId userId, SponsorTier tier)
    {
        RemoveSponsorPrivate(userId, tier);
    }

    private async void RemoveSponsorPrivate(NetUserId userId, SponsorTier tier)
    {
        await _serverDbManager.ChangeAdditionalSponsorTier(userId, tier, remove: true);
        _sawmill.Info("SponsorTier {tier} removed from {userId}", tier.ID, userId);
    }

    private void AddUniqueItems<T>(List<T> target, List<T> source)
    {
        var set = new HashSet<T>(target);
        foreach (var item in source)
        {
            if (set.Add(item))
                target.Add(item);
        }
    }
}
