﻿using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Saint.Reagent.Events;

/**
 * Событие прокидывается, когда святая вода выпита.
 * Может быть перехвачено системами или отменено
 */
public sealed class OnSaintWaterDrinkEvent : CancellableEntityEventArgs
{
    public EntityUid Target;
    public FixedPoint2 SaintWaterAmount;

    public OnSaintWaterDrinkEvent(EntityUid target, FixedPoint2 saintWaterAmount)
    {
        Target = target;
        SaintWaterAmount = saintWaterAmount;
    }
}
