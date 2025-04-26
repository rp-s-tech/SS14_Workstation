﻿using System.Collections.Generic;
using Content.Server.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Saint.Reagent;

public sealed class SaintWaterSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;

    private readonly DamageSpecifier _defaultDarkDamageHeal = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Fel", -10}
        }
    };

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReactiveComponent, OnSaintWaterDrinkEvent>(OnSaintWaterDrinkEvent);
        SubscribeLocalEvent<ReactiveComponent, OnSaintWaterFlammableEvent>(OnSaintWaterFlammableEvent);
    }

    private void OnSaintWaterDrinkEvent(EntityUid uid, ReactiveComponent component, OnSaintWaterDrinkEvent ev)
    {
        if (ev.Cancelled)
            return;

        _damageable.TryChangeDamage(ev.Target, _defaultDarkDamageHeal);
    }

    private void OnSaintWaterFlammableEvent(EntityUid uid, ReactiveComponent component, OnSaintWaterFlammableEvent ev)
    {
        if (ev.Cancelled)
            return;

        _damageable.TryChangeDamage(ev.Target, _defaultDarkDamageHeal / 2);
    }
}
