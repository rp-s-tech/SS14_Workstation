using Content.Shared.Objectives.Components;
using Robust.Shared.Random;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.StealSpecials;

public sealed partial class StealSpecialsConditionSystem : BasePirateObjective<StealSpecialsConditionComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void OnAssigned(Entity<StealSpecialsConditionComponent> entity, ref GroupObjectiveAssignedEvent args)
    {
        var targetList = new List<EntityUid>();
        var alreadyAssignedTargets = GetAssignedTargets();
        var query = EntityQueryEnumerator<StealTargetComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (!entity.Comp.StealTargetGroups.Contains(component.StealGroup)) continue;
            if (alreadyAssignedTargets.Contains(uid)) continue;
            targetList.Add(uid);
        }
        _random.Shuffle(targetList);
        entity.Comp.ObjectiveItem = _random.Pick(targetList);
    }

    protected override void CheckObjectiveCompleted(Entity<StealSpecialsConditionComponent> entity,
        ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;
        if (!entity.Comp.ObjectiveItem.HasValue) return;
        if (GetOutpost(entity) is not { Valid: true } outpost) return;
        if (Transform(entity.Comp.ObjectiveItem.Value).GridUid != outpost) return;
        args.Progress = 1f;
    }

    private List<EntityUid> GetAssignedTargets()
    {
        var query = EntityQueryEnumerator<StealSpecialsConditionComponent>();
        var list = new List<EntityUid>();
        while (query.MoveNext(out var component))
        {
            if (!component.ObjectiveItem.HasValue) continue;
            list.Add(component.ObjectiveItem.Value);
        }
        return list;
    }
}
