using Content.Server.Explosion.Components;
using Content.Shared.Explosion.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.NPC.Systems;

namespace Content.Server.Explosion.EntitySystems;

public sealed partial class TriggerSystem
{
    [Dependency] private readonly NpcFactionSystem _factionSystem = default!;

    private void InitializeOnEquipFaction()
    {
        SubscribeLocalEvent<OnEquipFactionTriggerComponent, GotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<OnEquipFactionTriggerComponent, GotUnequippedEvent>(OnUnequip);
    }

    private void OnEquip(EntityUid uid, OnEquipFactionTriggerComponent component, GotEquippedEvent args)
    {
        if (_factionSystem.ContainsFaction(component.Faction.Id, args.Equipee))
            return;

        HandleTimerTrigger(
            uid,
            args.Equipee,
            component.Delay,
            component.BeepInterval,
            component.InitialBeepDelay,
            component.BeepSound
        );

        _popupSystem.PopupEntity("Процесс самоуничтожения запущен", uid);
    }

    private void OnUnequip(EntityUid uid, OnEquipFactionTriggerComponent component, GotUnequippedEvent args)
    {
        if (!HasComp<ActiveTimerTriggerComponent>(uid))
            return;

        if (!component.StopOnUnequip)
            return;

        RemComp<ActiveTimerTriggerComponent>(uid);
        _popupSystem.PopupEntity("Самоуничтожение остановлено", uid);
    }


}
