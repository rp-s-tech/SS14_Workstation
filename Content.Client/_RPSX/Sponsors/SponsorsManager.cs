using System.Diagnostics.CodeAnalysis;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Content.Shared.RPSX.Patron;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Client.RPSX.Sponsors;

public sealed class SponsorsManager : ISponsorsManager
{
    [Dependency] private readonly IClientNetManager _netMgr = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private AllSponsorInfo? _tier;

    public void Initialize()
    {
        _netMgr.RegisterNetMessage<MsgSponsorInfo>(msg =>
            {
                _tier = null;
                if (msg.TierId != null)
                {
                    _tier = msg.TierId;
                }
            }
        );
    }

    public bool TryGetSponsorTier([NotNullWhen(true)] out AllSponsorInfo? sponsor)
    {
        sponsor = _tier;
        return _tier != null;
    }

    public bool IsJobAvailable(JobPrototype job)
    {
        var isWhiteListEnabled = _cfg.GetCVar(CCVars.GameRoleWhitelist);
        if (isWhiteListEnabled && job is { SponsorIgnore: false })
            return false;

        return _tier?.RoleTimeByPass == true;
    }

    public bool IsUserHasRoleTimeByPass(NetUserId userId)
    {
        return _tier?.RoleTimeByPass == true;
    }
}
