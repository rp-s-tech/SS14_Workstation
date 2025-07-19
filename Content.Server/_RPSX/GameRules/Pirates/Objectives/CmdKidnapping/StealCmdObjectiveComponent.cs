namespace Content.Server.RPSX.GameRules.Pirates.Objectives.CmdKidnapping;

[RegisterComponent]
public sealed partial class StealCmdObjectiveComponent : Component
{
    [DataField]
    public EntityUid Target;
}
