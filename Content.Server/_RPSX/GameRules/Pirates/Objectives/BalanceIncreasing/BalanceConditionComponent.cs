namespace Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;

[RegisterComponent]
public sealed partial class BalanceConditionComponent : Component
{
    [DataField]
    public int Goal;
}
