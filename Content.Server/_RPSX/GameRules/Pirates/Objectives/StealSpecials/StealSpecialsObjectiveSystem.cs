using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.GameRules.Pirates;
using Robust.Shared.Random;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.StealSpecials;

public sealed partial class StealSpecialsObjectiveSystem : BasePirateObjective<StealSpecialsObjectiveComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void OnMapInit(Entity<StealSpecialsObjectiveComponent> entity, ref MapInitEvent args)
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

    protected override void CheckObjectiveCompleted(Entity<StealSpecialsObjectiveComponent> entity,
        ref CheckObjectiveEvent args)
    {
        args.Completed = false;
        if (!entity.Comp.ObjectiveItem.HasValue) return;
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        if (progressComponent.PiratesOutpostMap is not { } outpost) return;
        if (Transform(entity.Comp.ObjectiveItem.Value).MapUid != outpost) return;
        args.Completed = true;
    }

    private List<EntityUid> GetAssignedTargets()
    {
        var query = EntityQueryEnumerator<StealSpecialsObjectiveComponent>();
        var list = new List<EntityUid>();
        while (query.MoveNext(out var component))
        {
            if (!component.ObjectiveItem.HasValue) continue;
            list.Add(component.ObjectiveItem.Value);
        }
        return list;
    }
}
