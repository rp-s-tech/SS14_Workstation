using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.GameRules.Pirates;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives;

public abstract partial class BasePirateObjective<T> : EntitySystem where T : Component
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<T, GroupObjectiveAssignedEvent>(OnAssigned);
        SubscribeLocalEvent<T, ObjectiveGetProgressEvent>(CheckObjectiveCompleted);
    }

    protected virtual void OnAssigned(Entity<T> entity, ref GroupObjectiveAssignedEvent args) { }
    protected abstract void CheckObjectiveCompleted(Entity<T> entity, ref ObjectiveGetProgressEvent args);

    protected EntityUid? GetOutpost(Entity<T> entity)
    {
        if (!TryComp<PirateObjectiveComponent>(entity, out var objective)) return null;
        if (!TryComp<PiratesProgressComponent>(objective.PiratesProgress, out var progressComponent)) return null;
        if (progressComponent.PiratesShuttle is not { } outpost) return null;
        return outpost;
    }
}
