using Content.Shared.RPSX.GameRules.Pirates;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives;

[RegisterComponent]
public sealed partial class PirateObjectiveComponent : BasePirateComponent
{
    [DataField]
    public int Reward;

    [DataField]
    public bool RewardGiven;

    [DataField]
    public string Priority = "additional";

    [DataField]
    public float Completion = 0f;
}
