using Robust.Shared.Serialization;
using Content.Shared.RPSX.Bank.Prototypes;

namespace Content.Shared.RPSX.Bank.PDA;

[NetSerializable, Serializable]
public sealed class ShopCartridgeInterfaceState : BoundUserInterfaceState
{
    public string OwnerName;
    public int Balance;
    public string StationName;
    public NetEntity LoaderUid;

    public ShopCartridgeInterfaceState(string ownerName, int balance, string stationName, NetEntity loaderUid)
    {
        OwnerName = ownerName;
        Balance = balance;
        StationName = stationName;
        LoaderUid = loaderUid;
    }
}

[Serializable, NetSerializable]
public sealed class ShopBuyMessage : BoundUserInterfaceMessage
{
    public string Requester;
    public Dictionary<StoreProductPrototype, int> BuyedProducts = new();

    public ShopBuyMessage(string requester, Dictionary<StoreProductPrototype, int> buyedProducts)
    {
        Requester = requester;
        BuyedProducts = buyedProducts;
    }
}
