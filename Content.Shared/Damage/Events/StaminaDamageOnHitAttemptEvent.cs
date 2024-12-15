using Content.Shared.Inventory;

namespace Content.Shared.Damage.Events;

/// <summary>
/// Attempting to apply stamina damage on entity.
/// </summary>
[ByRefEvent]
public record struct StaminaDamageOnHitAttemptEvent(bool Cancelled);

[ByRefEvent]
public record struct StaminaDamageModifyEvent(float Damage) : IInventoryRelayEvent
{
    SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING;
}
