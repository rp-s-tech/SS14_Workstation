using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.RPSX.DarkForces.Narsi.Abilities.Events;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Content.Shared.Weapons.Melee;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Spawners;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities;

public sealed partial class NarsiCultistAbilitiesSystem
{
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
    private void InitializeGhostWeapon()
    {
        SubscribeLocalEvent<NarsiCultistComponent, NarsiCultistGhostWeaponEvent>(OnGhostWeaponEvent);
    }

    private void OnGhostWeaponEvent(EntityUid uid, NarsiCultistComponent component, NarsiCultistGhostWeaponEvent args)
    {
        if (args.Handled)
            return;

        var level = _progressSystem.GetAbilityLevel(GhostWeaponAction);
        var duration = level switch
        {
            1 => 15f,
            2 => 30f,
            _ => 60f
        };

        var userCoords = Transform(uid).Coordinates;
        var ghostedAxe = Spawn("NarsiCultGhostAxe", userCoords);
        if (! _handsSystem.TryPickupAnyHand(uid, ghostedAxe))
        {
            _popupSystem.PopupClient("Ваши руки заняты...", uid, uid, PopupType.Medium);
            QueueDel(ghostedAxe);
            return;
        }

        var timedDespawn = EnsureComp<TimedDespawnComponent>(ghostedAxe);
        timedDespawn.Lifetime = duration;

        OnCultistAbility(uid, args);

        args.Handled = true;
    }
}
