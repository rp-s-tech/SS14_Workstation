using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.GameRules.Pirates;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class PiratesProgressComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesGamePlay GamePlay = PiratesGamePlay.Silent;

    [ViewVariables(VVAccess.ReadOnly)]
    public PiratesWinState PiratesWinState = PiratesWinState.Neutral;

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid TargetStation;

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid? PiratesOutpostMap;

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public int Balance = 0;

    [ViewVariables(VVAccess.ReadOnly)]
    public Dictionary<string, int> ObjectivesToSpawn = new();

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public List<EntityUid> Objectives = new();

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesSpawnTime = TimeSpan.Zero;

    [DataField, AutoPausedField]
    public TimeSpan ObjectivesCheckTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan ObjectivesCheckThreshold = TimeSpan.FromSeconds(10);

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool AreObjectivesCompleted;

    [DataField, AutoNetworkedField]
    public EntityUid CompOwner;
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
