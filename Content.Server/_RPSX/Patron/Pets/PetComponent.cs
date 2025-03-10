namespace Content.Server.RPSX.Patron.Pets;

[RegisterComponent]
public sealed partial class PetComponent : Component
{
    [DataField]
    public EntityUid? PetOwner;
}
