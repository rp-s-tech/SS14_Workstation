using Content.Shared.Chemistry.Reagent;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public record struct OnEntityStomachUpdated(ReagentQuantity Quantity);
