using Robust.Shared.Prototypes;

namespace Content.Server._RPSX.Actions;

[RegisterComponent]
public sealed partial class ActionGiveComponent : Component
{
    [DataField(required: true)]
    public HashSet<EntProtoId> Actions = new();
}
