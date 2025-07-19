using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.GameRules.Pirates;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class PiratesProgressComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid PiratesProgress;

    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesGamePlay GamePlay = PiratesGamePlay.Silent;

    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesWinState PiratesWinState = PiratesWinState.Neutral;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid TargetStation;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid? PiratesOutpost;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public int Balance = 0;

    [ViewVariables(VVAccess.ReadOnly)]
    public Dictionary<string, int> ObjectivesToSpawn = new();

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public List<EntityUid> Objectives = new();

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesSpawnTime = TimeSpan.Zero;

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesCheckTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan ObjectivesCheckThreshold = TimeSpan.FromSeconds(10);

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool AreObjectivesCompleted;
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
}
