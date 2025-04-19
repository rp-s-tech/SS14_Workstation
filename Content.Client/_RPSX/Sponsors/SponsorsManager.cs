using System.Diagnostics.CodeAnalysis;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Content.Shared.RPSX.Patron;
using Content.Shared.RPSX.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Client.RPSX.Sponsors;

public sealed class SponsorsManager : ISponsorsManager
{
    [Dependency] private readonly IClientNetManager _netMgr = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private SponsorTier? _tier;

    public void Initialize()
    {
        _netMgr.RegisterNetMessage<MsgSponsorInfo>(msg =>
            {
                if (msg.TierId == null || !_prototype.TryIndex(msg.TierId, out _tier))
                {
                    _tier = null;
                }
            }
        );
    }

    public bool TryGetSponsorTier([NotNullWhen(true)] out SponsorTier? sponsor)
    {
        sponsor = _tier;
        return _tier != null;
    }

    public bool IsJobAvailable(JobPrototype job)
    {
        var isWhiteListEnabled = _cfg.GetCVar(CCVars.GameRoleWhitelist);
        if (isWhiteListEnabled && job is { Whitelisted: true, SponsorIgnoreWhitelist: false })
            return false;

        return _tier?.RoleTimeByPass == true;
    }

    public bool IsUserHasRoleTimeByPass(NetUserId userId)
    {
        return _tier?.RoleTimeByPass == true;
    }
}
