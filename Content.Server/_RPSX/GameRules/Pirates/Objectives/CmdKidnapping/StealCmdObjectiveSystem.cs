using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Revolutionary.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Random;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.CmdKidnapping;

public sealed partial class StealCmdObjectiveSystem : BasePirateObjective<StealCmdObjectiveComponent>
{
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void CheckObjectiveCompleted(Entity<StealCmdObjectiveComponent> entity, ref CheckObjectiveEvent args)
    {
        args.Completed = false;
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        if (progressComponent.PiratesOutpost is not { } outpost) return;
        if (Transform(entity.Comp.Target).MapID != Transform(outpost).MapID) return;
        args.Completed = true;
    }

    protected override void OnMapInit(Entity<StealCmdObjectiveComponent> entity, ref MapInitEvent args)
    {
        var allHumans = GetAliveHumans().Where(c => !HasComp<PirateComponent>(c)).ToHashSet();
        if (!allHumans.Any()) return;

        var allHeads = new HashSet<EntityUid>();
        foreach (var person in allHumans)
        {
            if (HasComp<CommandStaffComponent>(person))
                allHeads.Add(person);
        }

        if (!allHeads.Any()) allHeads = allHumans;
        var targets = allHumans.Where(c => !GetAssignedTargets().Contains(c));
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

    public HashSet<EntityUid> GetAliveHumans(EntityUid? exclude = null)
    {
        var allHumans = new HashSet<EntityUid>();
        // HumanoidAppearanceComponent is used to prevent mice, pAIs, etc from being chosen
        var query = EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent>();
        while (query.MoveNext(out var uid, out var mobState, out _))
        {
            // the player needs to have a mind and not be the excluded one +
            // the player has to be alive
            if (!_mind.TryGetMind(uid, out var mind, out var mindComp) || mind == exclude || !_mobState.IsAlive(uid, mobState))
                continue;

            allHumans.Add(uid);
        }

        return allHumans;
    }
}
