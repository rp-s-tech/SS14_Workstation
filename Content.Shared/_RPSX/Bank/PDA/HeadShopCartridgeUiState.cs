using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Bank.PDA;

[NetSerializable, Serializable]
public sealed class HeadShopCartridgeInterfaceState : BoundUserInterfaceState
{
    public string OwnerName;
    public int Capacity;
    public int Balance;
    public NetEntity Owner;
    public string StationName;

    public HeadShopCartridgeInterfaceState(string ownerName, int capacity, int balance, NetEntity owner, string stationName)
    {
        OwnerName = ownerName;
        Capacity = capacity;
        Balance = balance;
        Owner = owner;
        StationName = stationName;
    }
}
