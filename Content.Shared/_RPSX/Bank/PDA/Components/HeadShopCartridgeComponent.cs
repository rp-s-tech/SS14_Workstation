using Content.Shared.Cargo.Prototypes;

namespace Content.Shared.RPSX.Bank.PDA.Components;

[RegisterComponent]
public sealed partial class HeadShopCartridgeComponent : Component
{
    /// <summary>
    /// All of the <see cref="CargoProductPrototype.Group"/>s that are supported.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public List<string> AllowedGroups = new() { "market" };
}
