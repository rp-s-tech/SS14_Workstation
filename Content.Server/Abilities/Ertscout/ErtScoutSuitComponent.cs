using Robust.Shared.Audio;

namespace Content.Server.Abilities.ErtScout;

[RegisterComponent]
public sealed partial class ErtScoutSuitComponent : Component
{
    [DataField]
    public SoundSpecifier EnableSound = new SoundPathSpecifier("/Audio/ERTRecon/enable.ogg");

    [DataField]
    public SoundSpecifier DisableSound = new SoundPathSpecifier("/Audio/ERTRecon/disable.ogg");
    /// <summary>
    /// Battery charge used passively, in watts. Will last 1000 seconds on a small-capacity power cell.
    /// </summary>
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float PassiveWattage = 0.36f;

    /// <summary>
    /// Battery charge used while cloaked, stacks with passive. Will last 200 seconds while cloaked on a small-capacity power cell.
    /// </summary>
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float CloakWattage = 1.44f;
}
