using Content.Shared.Actions;
using Content.Shared.Inventory;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.Eye.NightVision.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class NVGComponent : Component
{
    [DataField]
    public EntProtoId<InstantActionComponent> ActionProto = "NVToggleAction";

    [DataField]
    public EntityUid? ActionContainer;

    [DataField]
    public SlotFlags SlotFlags = SlotFlags.EYES;
}
