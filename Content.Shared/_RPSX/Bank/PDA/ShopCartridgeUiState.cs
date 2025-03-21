using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Bank.PDA;

[NetSerializable, Serializable]
public sealed class ShopCartridgeInterfaceState : BoundUserInterfaceState
{
    public string OwnerName;
    public int Balance;
    public NetEntity Owner;
    public string StationName;
    public NetEntity LoaderUid;

    public ShopCartridgeInterfaceState(string ownerName, int balance, NetEntity owner, string stationName, NetEntity loaderUid)
    {
        OwnerName = ownerName;
        Balance = balance;
        Owner = owner;
        StationName = stationName;
        LoaderUid = loaderUid;
    }
}
