using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Cult.Runes;

[Serializable, NetSerializable]
public sealed partial class SummonNarsiRuneDoAfterEvent : DoAfterEvent
{
    public SummonNarsiRuneDoAfterEvent()
    {
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}
