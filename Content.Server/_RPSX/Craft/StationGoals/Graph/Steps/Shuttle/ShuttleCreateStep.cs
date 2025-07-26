using System.Collections.Generic;
using Content.Server.Construction.Components;
using Content.Server.GameTicking;
using Content.Server.RPSX.Utils;
using Content.Shared.Construction.Components;
using Content.Shared.UserInterface;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Shuttle;

[ImplicitDataDefinitionForInheritors]
public sealed partial class ShuttleCreateStep : Step
{
    [DataField("shuttlePath", serverOnly: true)]
    private string ShuttlePath = string.Empty;
    [DataField("undestroyable", serverOnly: true)]
    private bool Undestroyable = false;
    private MapId MapId = MapId.Nullspace;
    private EntityUid ShuttleUid = EntityUid.Invalid;

    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
        var mapManager = entitySystemManager.GetEntitySystem<SharedMapSystem>();
        var mapLoaderSystem = entitySystemManager.GetEntitySystem<MapLoaderSystem>();
        var gameTicker = entitySystemManager.GetEntitySystem<GameTicker>();

        (MapId, ShuttleUid) = ShuttleUtils.CreateShuttleOnNewMap(mapManager, mapLoaderSystem, entityManager, ShuttlePath);
        if (MapId == MapId.Nullspace || ShuttleUid == EntityUid.Invalid)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interrupted Map or ShuttleUid is not valid");
            return ExecuteState.Interrupted;
        }

        if (Undestroyable)
        {
            MakeShuttleUnDestroyable(entityManager, entitySystemManager);
        }

        results[StepDataKey.MAP_ID] = MapId;
        results[StepDataKey.SHUTTLE_UID] = ShuttleUid;

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }
    private void MakeShuttleUnDestroyable(IEntityManager entityManager, IEntitySystemManager entitySystemManager)
    {
        var entityLookupSystem = entitySystemManager.GetEntitySystem<EntityLookupSystem>();
        var entititesToDisableDestory = entityLookupSystem.GetEntitiesIntersecting(ShuttleUid);

        foreach (var entity in entititesToDisableDestory)
        {
            entityManager.RemoveComponent<ActivatableUIComponent>(entity);
            entityManager.RemoveComponent<ConstructionComponent>(entity);
            entityManager.RemoveComponent<AnchorableComponent>(entity);
        }
    }

    public override void Cleanup()
    {
        var mapManager = IoCManager.Resolve<IMapManager>();
        var entityManager = IoCManager.Resolve<IEntityManager>();

        if (MapId != MapId.Nullspace && mapManager.MapExists(MapId))
            mapManager.DeleteMap(MapId);

        if (ShuttleUid != EntityUid.Invalid && entityManager.EntityExists(ShuttleUid))
            entityManager.QueueDeleteEntity(ShuttleUid);

        MapId = MapId.Nullspace;
        ShuttleUid = EntityUid.Invalid;
    }
}
