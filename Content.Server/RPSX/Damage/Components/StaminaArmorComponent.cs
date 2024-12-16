namespace Content.Server.RPSX.Damage.Components;

[RegisterComponent]
public sealed partial class StaminaArmorComponent : Component
{
    [DataField(required:true)]
    public float Coefficient = 1f;
}
