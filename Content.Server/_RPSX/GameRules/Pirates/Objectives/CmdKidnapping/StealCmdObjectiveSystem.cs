using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Revolutionary.Components;
using Robust.Shared.Random;
using Content.Server.Objectives;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.CmdKidnapping;

public sealed partial class StealCmdObjectiveSystem : BasePirateObjective<StealCmdObjectiveComponent>
{
    [Dependency] private readonly ObjectivesSystem _objectives = default!;

    protected override void CheckObjectiveCompleted(Entity<StealCmdObjectiveComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return;
        if (progressComponent.PiratesShuttle is not { } outpost) return;
        var allHeads = _objectives.GetAliveHumans()
            .Where(c => HasComp<CommandStaffComponent>(c))
            .ToHashSet();
        var kidnappedHeads = _objectives.GetAliveHumans()
            .Where(c => HasComp<CommandStaffComponent>(c)
                && Transform(c).GridUid == outpost)
            .ToHashSet();
        args.Progress = kidnappedHeads.Count / allHeads.Count;
    }
}
