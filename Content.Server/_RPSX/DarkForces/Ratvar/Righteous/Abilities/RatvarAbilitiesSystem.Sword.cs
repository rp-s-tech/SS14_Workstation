﻿using Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Weapons;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.RPSX.DarkForces.Ratvar.UI;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities;

public sealed partial class RatvarAbilitiesSystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;

    private const string SwordsmanEnchantment = "RatvarSwordsSwordsman";
    private const string BloodshedEnchantment = "RatvarSwordBloodshed";

    private void InitializeSword()
    {
        SubscribeLocalEvent<RatvarSwordComponent, MeleeHitEvent>(OnSwordMeleeHit);
        SubscribeLocalEvent<RatvarSwordComponent, GetMeleeAttackRateEvent>(OnGetMeleeAttackRate);
    }

    private void OnGetMeleeAttackRate(EntityUid uid, RatvarSwordComponent component, ref GetMeleeAttackRateEvent args)
    {
        if (!IsEnchantmentActive(args.Weapon, out var enchantment) || enchantment != SwordsmanEnchantment)
            return;

        args.Multipliers += 1.5f;
    }

    private void OnSwordMeleeHit(EntityUid uid, RatvarSwordComponent component, MeleeHitEvent args)
    {
        if (!IsEnchantmentActive(args.Weapon, out var enchantment))
            return;

        switch (enchantment)
        {
            case BloodshedEnchantment:
                foreach (var player in args.HitEntities)
                {
                    if (!HasComp<BloodstreamComponent>(player))
                        continue;

                    _bloodstreamSystem.TryModifyBleedAmount(player, -100);
                }
                break;
            case SwordsmanEnchantment:
                args.BonusDamage = component.SwordsmanDamage;
                break;
        }
    }
}
