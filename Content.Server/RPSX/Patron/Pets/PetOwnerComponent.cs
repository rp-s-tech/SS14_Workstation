using Content.Shared.RPSX.Patron.Pets;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Patron.Pets;

[RegisterComponent]
public sealed partial class PetOwnerComponent : Component
{
    [DataField(required: true)]
    public EntProtoId PetId;

    [DataField]
    public EntityUid? Pet;

    [DataField]
    public string? PetName;

    [DataField]
    public bool IsSponsorPet;

    [DataField]
    public bool OnePetInGame;

    [DataField]
    public Dictionary<EntProtoId, PetActionState> Actions = new();

    [DataField]
    public PetOrderType CurrentOrder = PetOrderType.Follow;
}

[ImplicitDataDefinitionForInheritors]
public sealed partial class PetActionState
{
    [DataField]
    public EntityUid? ActionUid;

    [DataField]
    public bool Available = true;
}
