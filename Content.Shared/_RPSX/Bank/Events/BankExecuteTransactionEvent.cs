using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.Network;

namespace Content.Shared.RPSX.Bank.Events;

public sealed class BankExecuteTransactionEvent : CancellableEntityEventArgs
{
    public EntityUid MobUid;
    public NetUserId UserId;
    public BankTransaction Transaction;

    public BankExecuteTransactionEvent(EntityUid mobUid, NetUserId userId, BankTransaction transaction)
    {
        MobUid = mobUid;
        UserId = userId;
        Transaction = transaction;
    }
}
