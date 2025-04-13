using Robust.Shared.Serialization;
using Content.Shared.RPSX.Bank.Prototypes;
using Content.Shared.StationRecords;
using Content.Shared.CartridgeLoader;

namespace Content.Shared.RPSX.Bank.PDA;

[NetSerializable, Serializable]
public sealed class ShopCartridgeInterfaceState : BoundUserInterfaceState
{
    public GeneralStationRecord Record;
    public int Balance;
    public NetEntity LoaderUid;
    public NetEntity Uid;
    public Dictionary<StoreProductPrototype, int> Basket;


    public ShopCartridgeInterfaceState(GeneralStationRecord record, int balance, NetEntity loaderUid, NetEntity uid, Dictionary<StoreProductPrototype, int> basket)
    {
        Record = record;
        Balance = balance;
        LoaderUid = loaderUid;
        Uid = uid;
        Basket = basket;
    }
}

[Serializable, NetSerializable]
public sealed class ShopBuyMessage : CartridgeMessageEvent
{
    public NetEntity LoaderUid;
    public GeneralStationRecord Record;

    public ShopBuyMessage(NetEntity loaderUid, GeneralStationRecord record)
    {
        LoaderUid = loaderUid;
        Record = record;
    }
}

[Serializable, NetSerializable]
public sealed class ShopUpdateMessage : CartridgeMessageEvent
{
    public StoreProductPrototype Product = new();
    public int Count;
    public NetEntity LoaderUid;

    public ShopUpdateMessage(NetEntity loaderUid, StoreProductPrototype product, int count)
    {
        Product = product;
        Count = count;
        LoaderUid = loaderUid;
    }
}
