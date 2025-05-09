using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.DarkForces.Saint.Chaplain.Events.Narsi;

[Serializable, NetSerializable]
public sealed partial class ChaplainNarsiExileDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}
