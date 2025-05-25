using Robust.Shared.Prototypes;

namespace Content.Server.SecretStation.Medical.Surgery.Bionic;

[RegisterComponent]
public sealed partial class BionicBodyPartComponent : Component
{
    [DataField("components")]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();
}
