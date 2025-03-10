using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.RPSX.Sponsors;

[Prototype("sponsorGhost")]
public sealed partial class SponsorGhostCategories : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField(required: true)]
    public string Name = default!;

    [DataField(required:true)]
    public SpriteSpecifier Icon = default!;
}
