namespace Content.Server.RPSX.GameRules.Pirates.Objectives;

public abstract partial class BasePirateObjective<T> : EntitySystem where T : Component
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<T, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<T, CheckObjectiveEvent>(CheckObjectiveCompleted);
    }

    protected virtual void OnMapInit(Entity<T> entity, ref MapInitEvent args) { }
    protected abstract void CheckObjectiveCompleted(Entity<T> entity, ref CheckObjectiveEvent args);
}
