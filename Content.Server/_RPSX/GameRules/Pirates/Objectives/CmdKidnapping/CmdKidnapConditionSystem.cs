using System.Linq;
using Content.Server.Revolutionary.Components;
using Content.Server.Objectives;
using Content.Shared.Objectives.Components;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.CmdKidnapping;

public sealed partial class CmdKidnapConditionSystem : BasePirateObjective<CmdKidnapConditionComponent>
{
    [Dependency] private readonly ObjectivesSystem _objectives = default!;

    protected override void CheckObjectiveCompleted(Entity<CmdKidnapConditionComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        if (GetOutpost(entity) is not { Valid: true } outpost) return;
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
