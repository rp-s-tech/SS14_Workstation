using System.Collections.Generic;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.RPSX.Utils;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Shuttle;

[ImplicitDataDefinitionForInheritors]
public sealed partial class ShuttleMoveToStation : Step
{
    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var shuttleUid = results.GetOrFallback<EntityUid>(StepDataKey.SHUTTLE_UID, EntityUid.Invalid);
        if (shuttleUid == EntityUid.Invalid)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interrupted shuttleUid Invalid");
            return ExecuteState.Interrupted;
        }

        var entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
        var shuttleSystem = entitySystemManager.GetEntitySystem<ShuttleSystem>();
        var gameTicker = entitySystemManager.GetEntitySystem<GameTicker>();
        var stationSystem = entitySystemManager.GetEntitySystem<StationSystem>();
        var mapManager = entitySystemManager.GetEntitySystem<SharedMapSystem>();
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var chatSystem = entitySystemManager.GetEntitySystem<ChatSystem>();


        var targetStation = ShuttleUtils.GetTargetStation(gameTicker, mapManager, entityManager);
        if (targetStation == EntityUid.Invalid)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interrupted targetStation Invalid");
            return ExecuteState.Interrupted;
        }

        var targetGrid = stationSystem.GetLargestGrid(entityManager.GetComponent<StationDataComponent>(targetStation)).GetValueOrDefault();

        var shuttleComponent = entityManager.EnsureComponent<ShuttleComponent>(shuttleUid);
        shuttleSystem.TryFTLDock(
            shuttleUid: shuttleUid,
            component: shuttleComponent,
            targetUid: targetGrid
        );

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }
}
