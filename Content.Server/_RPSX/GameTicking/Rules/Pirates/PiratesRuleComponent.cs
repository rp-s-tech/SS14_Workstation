using Content.Shared.NPC.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.RPSX.Server.GameTicking.Rules.Pirates;

[RegisterComponent]
public sealed partial class PiratesRuleComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public ProtoId<NpcFactionPrototype> Faction = "Syndicate";
}
