using Content.Shared.RPSX.Cult.Runes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Narsi.Runes.Components;

[RegisterComponent]
public sealed partial class NarsiRuneComponent : SharedNarsiRuneComponent
{
    [DataField("runeState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public NarsiRuneState RuneState = NarsiRuneState.Idle;
}

public enum NarsiRuneState
{
    Idle,
    InUse
}
