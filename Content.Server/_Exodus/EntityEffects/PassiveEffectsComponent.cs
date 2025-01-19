using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;

namespace Content.Server.Exodus.EntityEffects;

[RegisterComponent]
public sealed partial class PassiveEffectsComponent : Component
{
    [DataField]
    public FixedPoint2 Accumulator = 0;

    [DataField]
    public FixedPoint2 Rate = 1.0f;

    [DataField]
    public EntityEffect[] Effects = [];
}
