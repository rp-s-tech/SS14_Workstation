using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Cult.Runes;

[Serializable, NetSerializable]
public sealed partial class SpawnNarsiDoAfterEvent : DoAfterEvent
{
    public SpawnNarsiDoAfterEvent()
    {
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}
