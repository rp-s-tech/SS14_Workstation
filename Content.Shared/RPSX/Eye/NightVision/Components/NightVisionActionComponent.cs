using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.Eye.NightVision.Components;

[RegisterComponent]
public sealed partial class NightVisionActionComponent : Component
{
    [DataField]
    public EntityUid? ActionContainer;

    [DataField]
    public EntProtoId ActionProto = "SwitchNightVision";
}
