using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.DarkForces.Narsi.Buildings.Altar;

[Serializable, NetSerializable]
public sealed partial class NarsiRitualDoAftertEvent : DoAfterEvent
{
    public NarsiRitualDoAftertEvent()
    {

    }
    public override DoAfterEvent Clone()
    {
        return this;
    }
}
