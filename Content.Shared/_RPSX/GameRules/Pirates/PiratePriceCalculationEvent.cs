namespace Content.Shared.RPSX.GameRules.Pirates;

[ByRefEvent]
public record struct PiratePriceCalculationEvent()
{
    /// <summary>
    /// The total price of the entity.
    /// </summary>
    public double Price = 0;

    /// <summary>
    /// Whether this event was already handled.
    /// </summary>
    public bool Handled = false;
}
