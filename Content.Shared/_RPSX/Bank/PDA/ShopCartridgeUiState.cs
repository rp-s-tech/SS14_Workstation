using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Bank.PDA;

[NetSerializable, Serializable]
public sealed class ShopCartridgeInterfaceState : BoundUserInterfaceState
{
    public string OwnerName;
    public int Balance;
    public string StationName;

    public ShopCartridgeInterfaceState(string ownerName, int balance, string stationName)
    {
        OwnerName = ownerName;
        Balance = balance;
        StationName = stationName;
    }
}
