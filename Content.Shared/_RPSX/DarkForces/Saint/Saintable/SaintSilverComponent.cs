﻿using Content.Shared.Damage;

namespace Content.Shared.RPSX.DarkForces.Saint.Saintable;

[RegisterComponent]
public sealed partial class SaintSilverComponent : Component
{
    [DataField("damageOnCollide")]
    public DamageSpecifier DamageOnCollide = new();

    [DataField("pushOnCollide")]
    public bool PushOnCollide;
}
