using Content.Shared.Actions;
using Content.Shared.Inventory.Events;
using Content.Shared.RPSX.Eye.NightVision.Components;
using Robust.Shared.Audio.Systems;


namespace Content.Server.RPSX.Eyes.NightVision;

public sealed class NVGSystem : EntitySystem
{
    [Dependency] private readonly NightVisionSystem _nightvisionableSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NVGComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<NVGComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<NVGComponent, GetItemActionsEvent>(OnGetItemActions);
        SubscribeLocalEvent<NVGComponent, NVGInstanActionEvent>(OnNVGActionToggle);
    }


    private void OnGetItemActions(Entity<NVGComponent> ent, ref GetItemActionsEvent args)
    {
        if (ent.Comp.SlotFlags != args.SlotFlags)
            return;

        args.AddAction(ref ent.Comp.ActionContainer, ent.Comp.ActionProto);
    }

    private void OnEquipped(Entity<NVGComponent> ent, ref GotEquippedEvent args)
    {
        if ((args.SlotFlags & ent.Comp.SlotFlags) != ent.Comp.SlotFlags)
            return;

        EnsureComp<NightVisionComponent>(args.Equipee);
    }

    private void OnUnequipped(Entity<NVGComponent> ent, ref GotUnequippedEvent args)
    {
        if ((args.SlotFlags & ent.Comp.SlotFlags) != ent.Comp.SlotFlags)
            return;

        if (!TryComp<NightVisionComponent>(args.Equipee, out var nvcomp))
            return;

        _nightvisionableSystem.ToggleNightVision((args.Equipee, nvcomp));
        RemCompDeferred<NightVisionComponent>(args.Equipee);
    }

    private void OnNVGActionToggle(Entity<NVGComponent> ent, ref NVGInstanActionEvent args)
    {
        if (!TryComp<NightVisionComponent>(args.Performer, out var nightVisionComp))
            return;

        _nightvisionableSystem.ToggleNightVision((args.Performer, nightVisionComp));
        _audioSystem.PlayPvs(args.OnOffSound, ent);
    }
}
