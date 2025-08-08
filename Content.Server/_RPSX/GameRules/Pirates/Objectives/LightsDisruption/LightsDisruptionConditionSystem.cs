using Content.Server.Power.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.LightsDisruption;

public sealed partial class LightsDisruptionConditionSystem : BasePirateObjective<LightsDisruptionConditionComponent>
{
    [Dependency] private readonly PowerMonitoringConsoleSystem _powerMonitoring = default!;

    private const float RoguePowerConsumerThreshold = 100000;

    protected override void CheckObjectiveCompleted(Entity<LightsDisruptionConditionComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;
        if (GetProgress(entity) is not { Owner.Valid: true } piratesProgress) return;
        var load = _powerMonitoring.CalculateLoad(piratesProgress.Comp.TargetStation);
        if (load.Item1 / load.Item2 >= 0.3d)
        {
            args.Progress = load.Item1 / (load.Item2 - 0.3d * load.Item2);
            return;
        }    
        args.Progress = 1f;
    }
}