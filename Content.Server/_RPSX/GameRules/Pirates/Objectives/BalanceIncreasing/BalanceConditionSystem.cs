using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;

public sealed partial class BalanceConditionSystem : BasePirateObjective<BalanceConditionComponent>
{
    protected override void CheckObjectiveCompleted(Entity<BalanceConditionComponent> entity,
        ref ObjectiveGetProgressEvent args)
    {
        if (GetProgress(entity) is not { Owner.Valid: true } piratesProgress) return;
        var progress = piratesProgress.Comp.Balance / entity.Comp.Goal;
        args.Progress = progress >= 1f ? 1f : progress;
    }
}
