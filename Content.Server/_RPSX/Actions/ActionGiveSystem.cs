using Content.Shared.Actions;

namespace Content.Server._RPSX.Actions;

public sealed class ActionGiveSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ActionGiveComponent, MapInitEvent>(OnAutoLearnAction);
    }

    private void OnAutoLearnAction(Entity<ActionGiveComponent> ent, ref MapInitEvent args)
    {
        foreach (var action in ent.Comp.Actions)
        {
            _action.AddAction(ent, action);
        }
        RemCompDeferred<ActionGiveComponent>(ent);
    }
}
