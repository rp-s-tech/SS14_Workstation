using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Revolutionary.Components;
using Robust.Shared.Random;
using Content.Server.Objectives;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.CmdKidnapping;

public sealed partial class StealCmdObjectiveSystem : BasePirateObjective<StealCmdObjectiveComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ObjectivesSystem _objectives = default!;

    protected override void CheckObjectiveCompleted(Entity<StealCmdObjectiveComponent> entity, ref CheckObjectiveEvent args)
    {
        args.Completed = false;
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        if (progressComponent.PiratesOutpostMap is not { } outpost) return;
        if (Transform(entity.Comp.Target).MapUid != outpost) return;
        args.Completed = true;
    }

    protected override void OnMapInit(Entity<StealCmdObjectiveComponent> entity, ref MapInitEvent args)
    {
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        var assignedTargets = GetAssignedTargets();
        var allHumans = _objectives.GetAliveHumans()
            .Where(c => !HasComp<PirateComponent>(c)
                && Transform(c).MapID == Transform(progressComponent.TargetStation).MapID)
            .ToHashSet();
        allHumans.ExceptWith(assignedTargets);
        if (!allHumans.Any()) return;

        var allHeads = new HashSet<EntityUid>();
        foreach (var person in allHumans)
        {
            if (HasComp<CommandStaffComponent>(person))
                allHeads.Add(person);
        }

        if (!allHeads.Any()) allHeads = allHumans;
        entity.Comp.Target = _random.Pick(allHeads);
    }

    private HashSet<EntityUid> GetAssignedTargets()
    {
        var query = EntityQueryEnumerator<StealCmdObjectiveComponent>();
        var list = new HashSet<EntityUid>();
        while (query.MoveNext(out var component))
        {
            if (!component.Target.Valid) continue;
            list.Add(component.Target);
        }
        return list;
    }
}
