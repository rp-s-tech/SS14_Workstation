using Content.Server.Emp;
using Content.Server.Ninja.Events;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;

namespace Content.Server.Abilities.ErtScout;

public sealed class ErtScoutSuitSystem : EntitySystem
{
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ErtScoutSuitComponent, ContainerIsInsertingAttemptEvent>(OnSuitInsertAttempt);
        SubscribeLocalEvent<ErtScoutSuitComponent, ClothingGotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<ErtScoutSuitComponent, GotUnequippedEvent>(OnUnequipped);

        SubscribeLocalEvent<ErtScoutSuitComponent, ToggleClothingCheckEvent>(OnCloakCheck);
        SubscribeLocalEvent<ErtScoutSuitComponent, ItemToggleActivateAttemptEvent>(OnActivateAttempt);
        SubscribeLocalEvent<ErtScoutSuitComponent, ItemToggledEvent>(OnToggled);

    }

    private void OnToggled(Entity<ErtScoutSuitComponent> ent, ref ItemToggledEvent args)
    {
        if (args.Activated)
        {
            _audio.PlayEntity(ent.Comp.EnableSound, Filter.Pvs(ent, entityManager: EntityManager), ent, true);
        }
        else
        {
            _audio.PlayEntity(ent.Comp.DisableSound, Filter.Pvs(ent, entityManager: EntityManager), ent, true);
        }
    }

    private void OnActivateAttempt(Entity<ErtScoutSuitComponent> ent, ref ItemToggleActivateAttemptEvent args)
    {
        args.Cancelled = !HasComp<ErtScoutComponent>(args.User);
    }

    private void OnCloakCheck(Entity<ErtScoutSuitComponent> ent, ref ToggleClothingCheckEvent args)
    {
        args.Cancelled = !HasComp<ErtScoutComponent>(args.User);
    }

    private void OnSuitInsertAttempt(Entity<ErtScoutSuitComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        // this is for handling battery upgrading, not stopping actions from being added
        // if another container like ActionsContainer is specified, don't handle it
        if (TryComp<PowerCellSlotComponent>(ent, out var slot) && args.Container.ID != slot.CellSlotId)
            return;

        // no power cell for some reason??? allow it
        if (!_powerCell.TryGetBatteryFromSlot(ent, out var battery))
            return;

        // can only upgrade power cell, not swap to recharge instantly otherwise ninja could just swap batteries with flashlights in maints for easy power
        if (!TryComp<BatteryComponent>(args.EntityUid, out var inserting) || inserting.MaxCharge <= battery.MaxCharge)
        {
            args.Cancel();
        }

        // tell ninja abilities that use battery to update it so they don't use charge from the old one
        var user = Transform(ent).ParentUid;
        if (!HasComp<ErtScoutComponent>(user))
            return;

        var ev = new NinjaBatteryChangedEvent(args.EntityUid, ent);
        RaiseLocalEvent(ent, ref ev);
        RaiseLocalEvent(user, ref ev);
    }


    private void OnEmpAttempt(Entity<ErtScoutSuitComponent> ent, ref EmpAttemptEvent args)
    {
        args.Cancel();
    }

    private void OnUnequipped(Entity<ErtScoutSuitComponent> ent, ref GotUnequippedEvent args)
    {
        var user = args.Equipee;
        if (!TryComp<ErtScoutComponent>(user, out var scout))
            return;

        // mark the user as not wearing a suit
        AssignSuit(user, null, scout);
    }

    private void OnEquipped(Entity<ErtScoutSuitComponent> ent, ref ClothingGotEquippedEvent args)
    {
        var user = args.Wearer;
        if (!TryComp<ErtScoutComponent>(user, out var scout))
            return;

        ScoutEquippedSuit(ent, user, scout);
    }

    private void ScoutEquippedSuit(Entity<ErtScoutSuitComponent> ent, EntityUid user, ErtScoutComponent scout)
    {
        // mark the user as wearing this suit, used when being attacked among other things
        AssignSuit(user, ent);
    }

    private void AssignSuit(EntityUid uid, EntityUid? suit, ErtScoutComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) || comp.Suit == suit)
            return;

        comp.Suit = suit;
    }
}
