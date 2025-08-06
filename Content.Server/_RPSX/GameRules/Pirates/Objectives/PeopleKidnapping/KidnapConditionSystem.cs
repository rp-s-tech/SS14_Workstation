using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.PeopleKidnapping;

public sealed partial class KidnapConditionSystem : BasePirateObjective<KidnapConditionComponent>
{
    [Dependency] private readonly TargetObjectiveSystem _target = default!;

    protected override void CheckObjectiveCompleted(Entity<KidnapConditionComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;
        if (GetOutpost(entity) is not { Valid: true } outpost) return;
        if (!_target.GetTarget(entity, out var target)) return;
        if (Transform(target.Value).GridUid != outpost) return;
        args.Progress = 1f;
    }
}