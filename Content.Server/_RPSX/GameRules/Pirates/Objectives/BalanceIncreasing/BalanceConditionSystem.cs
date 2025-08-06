using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;

public sealed partial class BalanceConditionSystem : BasePirateObjective<BalanceConditionComponent>
{
    protected override void CheckObjectiveCompleted(Entity<BalanceConditionComponent> entity,
        ref ObjectiveGetProgressEvent args)
    {
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        var progress = progressComponent.Balance / entity.Comp.Goal;
        args.Progress = progress >= 1f ? 1f : progress;
    }
}
