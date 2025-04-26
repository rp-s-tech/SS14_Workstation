using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.GameRules.Vampire.Role.Events;

[ByRefEvent]
public record VampireAbilitySelectedEvent(string ActionId, int BloodRequired, EntProtoId? ReplaceId);
