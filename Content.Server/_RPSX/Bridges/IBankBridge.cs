using Content.Shared.RPSX.Bank.Transactions;
using FFMpegCore.Builders.MetaData;
using Robust.Shared.Network;

namespace Content.Server.RPSX.Bridges;

public interface IBankBridge
{
    BankTransaction CreateBuyTransaction(EntityUid uid, int price);

    bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction);
}

public sealed class StubBankBridge : IBankBridge
{
    public BankTransaction CreateBuyTransaction(EntityUid uid, int price)
    {
        return new BankTransaction(
            location: "Stub Location",
            type: BankTransactionType.Deposit,
            status: BankTransactionStatus.Success,
            balanceChangeType: BankBalanceChangeType.Income,
            amount: 1000
        );
    }

    public bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction)
    {
        return false;
    }
}
