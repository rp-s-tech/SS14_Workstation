using Content.RPSX.Shared.GameRules.Pirates;

namespace Content.RPSX.Server.GameRules.Pirates.Objectives.BalanceIncreasing;

public sealed partial class BalanceIncreasingObjectiveSystem : BasePirateObjective<BalanceIncreasingObjectiveComponent>
{
    protected override void CheckObjectiveCompleted(Entity<BalanceIncreasingObjectiveComponent> entity,
        ref CheckObjectiveEvent args)
    {
        args.Completed = false;
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        args.Completed = progressComponent.Balance > entity.Comp.Goal;
    }
}
