using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.GameRules.Pirates.Economics;

[NetSerializable, Serializable]
public sealed class PirateShopInterfaceState : BoundUserInterfaceState
{
    public List<ProtoId<PirateStuffPrototype>> Products;
    public int Balance;

    public PirateShopInterfaceState(List<ProtoId<PirateStuffPrototype>> products, int balance)
    {
        Products = products;
        Balance = balance;
    }
}

[Serializable, NetSerializable]
public sealed class PirateShopOrderMessage : BoundUserInterfaceMessage
{
    public EntProtoId ProductId;
    public int Amount;
    public int Cost;

    public PirateShopOrderMessage(EntProtoId productId, int amount, int cost)
    {
        ProductId = productId;
        Amount = amount;
        Cost = cost;
    }
}

[NetSerializable, Serializable]
public sealed class PiratePalletConsoleInterfaceState : BoundUserInterfaceState
{
    /// <summary>
    /// estimated apraised value of all the entities on top of pallets on the same grid as the console
    /// </summary>
    public int Appraisal;

    /// <summary>
    /// number of entities on top of pallets on the same grid as the console
    /// </summary>
    public int Count;

    /// <summary>
    /// are the buttons enabled
    /// </summary>
    public bool Enabled;

    public PiratePalletConsoleInterfaceState(int appraisal, int count, bool enabled)
    {
        Appraisal = appraisal;
        Count = count;
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public sealed class PiratePalletSellMessage : BoundUserInterfaceMessage { }

[Serializable, NetSerializable]
public sealed class PiratePalletAppraiseMessage : BoundUserInterfaceMessage { }

public enum PirateShopUiKey : byte
{
    Key,
}

public enum PiratePalletConsoleUiKey : byte
{
    Key,
}
