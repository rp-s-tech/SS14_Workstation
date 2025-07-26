using Content.Server.Cargo.Components;
using Robust.Shared.Random;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.ItemsSpawn;

[ImplicitDataDefinitionForInheritors]
public sealed partial class ArtifactSpawnStep : Step
{
    [DataField("artifactsSpawnerPrototype", serverOnly: true, required: true)]
    private string _artifactSpawnerPrototype = default!;

    [DataField("minArtifacts", serverOnly: true)]
    private int _minArtifacts = 3;

    [DataField("maxArtifacts", serverOnly: true)]
    private int _maxArtifacts = 6;

    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var shuttleUid = results.GetOrFallback(StepDataKey.SHUTTLE_UID, EntityUid.Invalid);
        if (shuttleUid == EntityUid.Invalid)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interrupted shuttleUid Invalid");
            return ExecuteState.Interrupted;
        }

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var artifactsCount = IoCManager.Resolve<IRobustRandom>().Next(_minArtifacts, _maxArtifacts);
        var counter = 0;

        foreach (var (comp, compXform) in entityManager.EntityQuery<CargoPalletComponent, TransformComponent>(true))
        {
            if (counter == artifactsCount)
                break;

            if (compXform.ParentUid != shuttleUid || !compXform.Anchored)
                continue;


            entityManager.SpawnEntity(_artifactSpawnerPrototype, compXform.Coordinates);
            counter++;
        }

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }
}
