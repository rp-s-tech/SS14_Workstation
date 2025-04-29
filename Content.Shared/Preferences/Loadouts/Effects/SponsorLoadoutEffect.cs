using System.Diagnostics.CodeAnalysis;
using Content.Shared.RPSX.Patron;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System.Linq;
using Content.Shared.RPSX.Sponsors;

namespace Content.Shared.Preferences.Loadouts.Effects;

/// <summary>
/// Only sponsor that have it can select this loadout
/// </summary>
public sealed partial class SponsorLoadoutEffect : LoadoutEffect
{
    public override bool Validate(HumanoidCharacterProfile profile,
        RoleLoadout loadout,
        LoadoutPrototype proto,
        ICommonSession? session,
        IDependencyCollection collection,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = null;

        if (session == null)
            return true;

        var sponsorProtos = GetPrototypes(session, collection);
        if (!sponsorProtos.Contains(proto.ID))
        {
            reason = FormattedMessage.FromMarkupOrThrow(Loc.GetString("loadout-sponsor-only"));
            return false;
        }

        return true;
    }

    public List<string> GetPrototypes(ICommonSession session, IDependencyCollection collection)
    {
        if (!collection.TryResolveType<ISponsorsManager>(out var sponsorsManager))
            return [];

        var net = collection.Resolve<INetManager>();

        if (net.IsClient)
        {
            var clientList = new List<string>();
            if (sponsorsManager.TryGetAdditionalSponsorTier(out var clientAdditionalSponsor))
                clientList.AddRange(clientAdditionalSponsor.AllowedLoadouts.Where(item => !clientList.Contains(item)));

            if (sponsorsManager.TryGetSponsorTier(out var clientSponsor))
                clientList.AddRange(clientSponsor.AllowedLoadouts.Where(item => !clientList.Contains(item)));

            return clientList;
        }

        var serverList = new List<string>();
        if (sponsorsManager.TryGetAdditionalSponsorTier(session.UserId, out var serverAsdditionalSponsor))
            serverList.AddRange(serverAsdditionalSponsor.AllowedLoadouts.Where(item => !serverList.Contains(item)));

        if (sponsorsManager.TryGetSponsorTier(session.UserId, out var serverSponsor))
            serverList.AddRange(serverSponsor.AllowedLoadouts.Where(item => !serverList.Contains(item)));

        return serverList;
    }
}
