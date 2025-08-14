using Content.Server.RPSX.GameRules.Pirates.Objectives;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;

[RegisterComponent]
public sealed partial class BalanceConditionComponent : BasePirateObjectiveComponent
{
    [DataField]
    public int Goal;
}
