using Content.Shared.Chemistry;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Saint.Reagent.Events;

/**
 * Вызывается, если у сущности есть Flammable в Reactive компоненте
 */
public sealed class OnSaintWaterFlammableEvent : CancellableEntityEventArgs
{
    public EntityUid Target;
    public FixedPoint2 SaintWaterAmount;
    public ReactionMethod? ReactionMethod;

    public OnSaintWaterFlammableEvent(EntityUid target, FixedPoint2 saintWaterAmount, ReactionMethod? reactionMethod)
    {
        Target = target;
        SaintWaterAmount = saintWaterAmount;
        ReactionMethod = reactionMethod;
    }
}
