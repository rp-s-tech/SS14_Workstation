using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Seal;

/// <summary>
/// Component using to make one time lock 
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedSealSystem))]
public sealed partial class SealComponent : Component
{
    [DataField("sealed"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Sealed = true;

    /// <summary>
    /// Will make station announcment on open 
    /// </summary>
    [DataField("willAnnounce"), ViewVariables(VVAccess.ReadWrite)]
    public bool WillAnnounce = false;

    /// <summary>
    /// Spawns entity on unseal or do nothing if empty
    /// </summary>

    [DataField("spawnOnUnseal"), ViewVariables(VVAccess.ReadWrite)]
    public string? SpawnOnUnseal;

    [DataField("breakOnEmag")]
    [AutoNetworkedField]
    public bool BreakOnEmag = true;

    [DataField("removeOnUnseal")]
    [AutoNetworkedField]
    public bool RemoveOnUnseal = true;

    [DataField("UnsealTime")]
    [AutoNetworkedField]
    public TimeSpan UnsealTime = TimeSpan.FromSeconds(5);

    [DataField("sealType")]
    [AutoNetworkedField]
    public SealLockType SealType = 0;

    [DataField]
    public string? NameToAccess;

    [DataField("unsealingSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier UnsealingSound = new SoundPathSpecifier("/Audio/Machines/door_lock_off.ogg")
    {
        Params = AudioParams.Default.WithVolume(-5f),
    };

    /// <summary>
    /// Title and Text of announcment if enabled
    /// </summary>
    [DataField("announceTitle")]
    public LocId? AnnounceTitle;

    [DataField("announceText")]
    public LocId? AnnounceText;

}

[ByRefEvent]
public record struct UnsealAttemptEvent(EntityUid User, bool Silent = false, bool Cancelled = false);

[ByRefEvent]
public readonly record struct UnsealedEvent();

[Serializable, NetSerializable]
public enum SealVisual : byte
{
    Sealed
}

[Serializable, NetSerializable]
public sealed partial class UnsealDoAfter : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}


