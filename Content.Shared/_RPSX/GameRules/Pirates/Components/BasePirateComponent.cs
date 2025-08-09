namespace Content.Shared.RPSX.GameRules.Pirates;

[RegisterComponent]
public abstract partial class BasePirateComponent : Component
{
    [DataField]
    public EntityUid PiratesProgress;
}
