using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.Sponsors;

[Prototype("sponsorTier")]
public sealed class SponsorTier : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public int AvailableItems;

    [DataField]
    public bool RoleTimeByPass;

    [DataField]
    public List<string> AllowedMarkings = [];

    [DataField]
    public List<string> AllowedLoadouts = [];

    [DataField]
    public List<string> AllowedSpecies = [];

    [DataField]
    public bool HavePriorityJoin;

    [DataField]
    public List<ProtoId<SponsorPetCategory>> PetCategories = [];

    [DataField]
    public List<ProtoId<SponsorGhostCategories>> Ghosts = [];

    [DataField]
    public string? OOCColor { get; set; }
}
