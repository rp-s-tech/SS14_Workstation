// Exodus
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Damage.Components;

/// <summary>
/// Add to an entity to paralyze it whenever it reaches critical amounts of Stamina DamageType.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState] // Exodus - Stamina Refactor | Remove AutoGenerateComponentPause
public sealed partial class StaminaComponent : Component
{
    // Exodus - Stamina Refactor | Remove Critical State

    /// <summary>
    /// How much stamina reduces per second.
    /// </summary>
    [DataField, AutoNetworkedField] // Exodus - Stamina Refactor | Remove VV
    public float Decay = 3f;

    /// <summary>
    /// How much time after receiving damage until stamina starts decreasing.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(1f); // Exodus - Stamina Refactor | Change to TimeSpan, decrease duration

    /// <summary>
    /// How much stamina damage this entity has taken.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float StaminaDamage;

    /// <summary>
    /// How much stamina damage is required to entire stam crit.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float CritThreshold = 100f;

    /// <summary>
    /// How long will this mob be stunned for?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan StunTime = TimeSpan.FromSeconds(3); // Exodus - Balance

    // Exodus - Stamina refactor - start

    /// <summary>
    /// Delay before a new stun can be applied
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan StunInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Save time of last stamina stun
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan LastStun = TimeSpan.Zero;

    /// <summary>
    /// Modify stamina damage when walking
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float DamageWalkingModify = 0.88f;
    // Exodus - end

    // Exodus - Stamina Refactor | Remove Next Update
    // Exodus - Stamina Refactor | Remove Stamina Alert

    // Exodus - Stamina Rework
    /// <summary>
    /// To avoid continuously updating our data we track the last time we updated so we can extrapolate our current stamina.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan LastDamage = TimeSpan.Zero;


    /// <summary>
    /// Some systems decays depends on stamina damage
    /// This is set threshold
    /// </summary>
    public float SetDangerThreshold => CritThreshold * 0.9f;

    /// <summary>
    /// Some systems decays depends on stamina damage
    /// This is reset threshold
    /// </summary>
    public float ResetDangerThreshold => CritThreshold * 0.85f;

    /// <summary>
    /// Some systems decays depends on stamina damage
    /// </summary>
    [AutoNetworkedField]
    public bool IsInDanger = false;

    public float DangerThreshold => IsInDanger ? ResetDangerThreshold : SetDangerThreshold;

    /// <summary>
    /// How much stamina reduces per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float BaseDecay = 10.0f;
    // Exodus - End
}
