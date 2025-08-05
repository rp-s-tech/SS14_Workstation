using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.GameRules.Pirates;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true), AutoGenerateComponentPause]
public sealed partial class PiratesProgressComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid CompOwner;

    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesGamePlay GamePlay = PiratesGamePlay.Silent;

    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesWinState PiratesWinState = PiratesWinState.None;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid TargetStation;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid? PiratesShuttle;

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? PiratesStartMap;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public HashSet<EntityUid> StartedPirates;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public int Balance = 0;

    [ViewVariables(VVAccess.ReadOnly)]
    public Dictionary<string, int> ObjectivesToSpawn = new();

    [DataField, AutoNetworkedField]
    public List<EntityUid> Objectives = new();

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesSpawnTime = TimeSpan.Zero;

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesCheckTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan ObjectivesCheckThreshold = TimeSpan.FromSeconds(10);

    [ViewVariables(VVAccess.ReadOnly)]
    public bool RoundCanBeEnded;
}

public enum PiratesGamePlay
{
    Silent,
    Loud,
}

public enum PiratesWinState
{
    PiratesMajor,
    PiratesMinor,
    Neutral,
    CrewMinor,
    CrewMajor,
    None,
}
