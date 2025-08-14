using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.GameRules.Pirates;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives;

public abstract partial class BasePirateObjective<T> : EntitySystem where T : BasePirateObjectiveComponent
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
        if (!TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent)) return null;
        if (progressComponent.PiratesShuttle is not { Valid: true } outpost) return null;
        return outpost;
    }

    protected Entity<PiratesProgressComponent>? GetProgress(Entity<T> entity)
    {
        if (!TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent)) return null;
        return (entity.Comp.PiratesProgress, progressComponent);
    }
}
