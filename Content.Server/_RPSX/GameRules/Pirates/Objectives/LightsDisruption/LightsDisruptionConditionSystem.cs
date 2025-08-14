using Content.Server.Power.EntitySystems;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.LightsDisruption;

public sealed partial class LightsDisruptionConditionSystem : BasePirateObjective<LightsDisruptionConditionComponent>
{
    [Dependency] private readonly PowerMonitoringConsoleSystem _powerMonitoring = default!;

    protected override void CheckObjectiveCompleted(Entity<LightsDisruptionConditionComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;
        if (GetProgress(entity) is not { Owner.Valid: true } piratesProgress)
            return;

        if (piratesProgress.Comp.TargetStation is not { Valid: true } targetStation)
            return;

        var load = _powerMonitoring.CalculateLoad(targetStation);
        if (load.Item1 / load.Item2 >= 0.3d)
        {
            var progress = load.Item1 / (load.Item2 - 0.3d * load.Item2);
            args.Progress = (float)progress;
            return;
        }
        args.Progress = 1f;
    }
}