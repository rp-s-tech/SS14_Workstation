using Content.Shared.RPSX.Bank.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.RPSX.Bank.PDA.Components;

[RegisterComponent]
public sealed partial class ShopCartridgeComponent : Component
{
    [DataField]
    public Dictionary<StoreProductPrototype, int> Basket = new();

    [DataField]
    public SoundSpecifier ErrorSound = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg");

    [DataField]
    public SoundSpecifier ConfirmSound = new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");
}
