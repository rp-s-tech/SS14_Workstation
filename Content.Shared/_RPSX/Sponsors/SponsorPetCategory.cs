using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.Sponsors;

[Prototype("sponsorPetCategory")]
public sealed class SponsorPetCategory : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public List<EntProtoId> Pets = [];
}
