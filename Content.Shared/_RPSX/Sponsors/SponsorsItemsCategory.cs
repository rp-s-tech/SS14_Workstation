using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.Sponsors;

[Prototype("sponsorsItemsCategory")]
public sealed class SponsorsItemsCategory : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public string Name = default!;

    [DataField]
    public List<EntProtoId> Items = new();
}
