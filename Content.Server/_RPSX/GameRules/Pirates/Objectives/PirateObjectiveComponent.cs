namespace Content.Server.RPSX.GameRules.Pirates.Objectives;

[RegisterComponent]
public sealed partial class PirateObjectiveComponent : Component
{
    [ViewVariables]
    public EntityUid PiratesProgress;

    [DataField]
    public int Reward;

    [DataField]
    public bool RewardGiven;
}
