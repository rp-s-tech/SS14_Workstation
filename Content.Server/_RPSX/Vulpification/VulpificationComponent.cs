using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.VulpificationVirus;

[RegisterComponent]
public sealed partial class VulpificationComponent : Component
{

    [DataField]
    public float SuccessChance = 0.4f;

    [DataField]
    public ProtoId<PolymorphPrototype> PolymorphPrototypeName = "AdminInfectedVulpkaninSmite";

}
