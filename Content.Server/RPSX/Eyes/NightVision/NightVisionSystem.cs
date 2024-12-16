using Content.Shared.Actions;
using Content.Shared.RPSX.Eye.NightVision.Components;
using Content.Shared.RPSX.Eye.NightVision.Events;
using Robust.Shared.Player;

namespace Content.Server.RPSX.Eyes.NightVision;

public sealed class NightVisionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NightVisionActionComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<NightVisionComponent, NVInstantActionEvent>(OnActionToggle);
    }

    private void OnComponentInit(Entity<NightVisionActionComponent> ent, ref ComponentInit args)
    {
        _actionsSystem.AddAction(ent, ref ent.Comp.ActionContainer, ent.Comp.ActionProto);
    }

    private void OnActionToggle(Entity<NightVisionComponent> ent, ref NVInstantActionEvent args)
    {
        ToggleNightVision(ent);
    }

    public void ToggleNightVision(Entity<NightVisionComponent> ent)
    {
        ent.Comp.IsNightVisionOn = !ent.Comp.IsNightVisionOn;
        Dirty(ent);

        if (!TryComp<ActorComponent>(ent, out var actorComponent))
            return;

        var ev = new NightVisionChangedEvent();
        RaiseNetworkEvent(ev, actorComponent.PlayerSession.Channel);
    }
}

