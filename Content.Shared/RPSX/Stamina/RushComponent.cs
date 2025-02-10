using Robust.Shared.GameStates;
using Robust.Shared.Timing;


namespace Content.Shared.RPSX.Stamina;

[RegisterComponent, NetworkedComponent]
public sealed partial class RushComponent : Component
{
    /// <summary>
    /// How much stamina spends for sprint per second
    /// </summary>
    [DataField]
    public float StaminaDrain = 20.0f;

    /// <summary>
    /// Speed modify
    /// </summary>
    [DataField]
    public float RushModify = 1.7f;

    /// <summary>
    /// Modify stamina damage to rushing
    /// </summary>
    [DataField]
    public float StaminaDamageModify = 1.22f;
}
