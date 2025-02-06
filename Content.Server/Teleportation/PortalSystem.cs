using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Mind.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.Map;
using Content.Server.RPSX.RandomTeleport; // RPSX - RandomTeleport Refactor | add using
using Content.Shared.Teleportation.Components; // RPSX - RandomTeleport Refactor | add using

namespace Content.Server.Teleportation;

public sealed class PortalSystem : SharedPortalSystem
{
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly RandomTeleportSystem _randomTeleport = default!; // RPSX - RandomTeleport Refactor | add dependency

    // TODO Move to shared
    protected override void LogTeleport(EntityUid portal, EntityUid subject, EntityCoordinates source,
        EntityCoordinates target)
    {
        if (HasComp<MindContainerComponent>(subject) && !HasComp<GhostComponent>(subject))
            _adminLogger.Add(LogType.Teleport, LogImpact.Low, $"{ToPrettyString(subject):player} teleported via {ToPrettyString(portal)} from {source} to {target}");
    }

    // RPSX - RandomTeleport Refactor - start
    protected override EntityCoordinates? GetRandomCoords(EntityUid portal, PortalComponent? component = null)
    {
        if (!Resolve(portal, ref component))
            return null;

        return _randomTeleport.GetRandomCoordinates(portal, component.MaxRandomRadius);
    }
    // RPSX - end
}
