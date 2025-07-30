using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;

public sealed partial class BalanceIncreasingObjectiveSystem : BasePirateObjective<BalanceIncreasingObjectiveComponent>
{
    protected override void CheckObjectiveCompleted(Entity<BalanceIncreasingObjectiveComponent> entity,
        ref ObjectiveGetProgressEvent args)
    {
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        args.Progress = progressComponent.Balance / entity.Comp.Goal;
    }
}
