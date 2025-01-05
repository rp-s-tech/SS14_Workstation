using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

    private readonly HttpClient _httpClient = new();

    private ISawmill _sawmill = default!;
    private string _apiUrl = string.Empty;

    private readonly Dictionary<NetUserId, string> _cachedSponsors = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(RPSXCCVars.SponsorsApiUrl, s => _apiUrl = s, true);

        _netMgr.RegisterNetMessage<MsgSponsorInfo>();

        _netMgr.Connecting += OnConnecting;
        _netMgr.Connected += OnConnected;
        _netMgr.Disconnect += OnDisconnect;
    }

    public bool TryGetSponsorTier(NetUserId userId, [NotNullWhen(true)] out SponsorTier? sponsorTier)
    {
        sponsorTier = null;
        return _cachedSponsors.TryGetValue(userId, out var tierId) && _prototype.TryIndex(tierId, out sponsorTier);
    }

    public bool IsJobAvailable(NetUserId userId, JobPrototype job)
    {
        var isWhiteListEnabled = _cfg.GetCVar(CCVars.GameRoleWhitelist);
        if (isWhiteListEnabled && job is { Whitelisted: true, SponsorIgnoreWhitelist: false })
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
        if (info?.TierId == null)
        {
            _cachedSponsors.Remove(e.UserId); // Remove from cache if sponsor expired
            return;
        }

        DebugTools.Assert(!_cachedSponsors.ContainsKey(e.UserId), "Cached data was found on client connect");

        _cachedSponsors[e.UserId] = info.TierId;
    }

    private void OnConnected(object? sender, NetChannelArgs e)
    {
        var tierId = _cachedSponsors.TryGetValue(e.Channel.UserId, out var sponsor) ? sponsor : null;
        if (sponsor is not null)
            Logger.Info(sponsor);
        else
            Logger.Info("failed to get sponsor tier");


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

    public async void AddSponsor(NetUserId userId, string tier)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return;

        var url = $"{_apiUrl}/sponsors/add";

        var postData = new List<KeyValuePair<string, string>>();
        postData.Add(new KeyValuePair<string, string>("userId", userId.ToString()));
        postData.Add(new KeyValuePair<string, string>("tier", tier));

        var content = new FormUrlEncodedContent(postData);
        var response = await _httpClient.PostAsync(url, content);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                _sawmill.Info("Sponsor Added {userId} {tier}", userId, tier);
                break;
            default:
                _sawmill.Error(
                    "Failed to add player sponsor: [{StatusCode}] {Response}",
                    response.StatusCode
                );
                break;
        }
    }

    public async void RemoveSponsor(NetUserId userId)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return;

        var url = $"{_apiUrl}/sponsors/remove";
        var postData = new List<KeyValuePair<string, string>>();
        postData.Add(new KeyValuePair<string, string>("userId", userId.ToString()));

        var content = new FormUrlEncodedContent(postData);
        var response = await _httpClient.PostAsync(url, content);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                _sawmill.Info("Sponsor Removed {userId}", userId);
                break;
            default:
                _sawmill.Error(
                    "Failed to remove player sponsor: [{StatusCode}] {Response}",
                    response.StatusCode
                );
                break;
        }
    }
}
