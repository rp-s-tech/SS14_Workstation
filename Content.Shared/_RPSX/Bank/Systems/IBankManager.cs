using Content.Shared.Mind;
using Content.Shared.RPSX.Bank.Events;
using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.Network;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.RPSX.Bank.Components;

namespace Content.Shared.RPSX.Bank.Systems;

public interface IBankManager
{
    public BankTransaction CreateDepositTransaction(EntityUid source, int amount);
    public BankTransaction CreateSalaryTransaction(int amount, BankSalarySource source);
    public BankTransaction CreateWithdrawTransaction(EntityUid source, int amount);
    public BankTransaction CreateBuyTransaction(EntityUid source, int amount);
    public bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction);
    public bool TryGetBankAccount(EntityUid mobUid, [NotNullWhen(true)] out BankAccountComponent? bank);
}
public abstract class BankManagerBase : IBankManager
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntitySystemManager _entSysManager = default!;

    public BankTransaction CreateDepositTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            location: _entityManager.GetEntityData(_entityManager.GetNetEntity(source)).Item2.EntityName,
            type: BankTransactionType.Deposit,
            status: BankTransactionStatus.Success,
            balanceChangeType: BankBalanceChangeType.Income,
            amount: amount
        );
    }

    public BankTransaction CreateSalaryTransaction(int amount, BankSalarySource source)
    {
        return new BankTransaction(
            location: $"{source}",
            type: BankTransactionType.Salary,
            status: BankTransactionStatus.Success,
            balanceChangeType: BankBalanceChangeType.Income,
            amount: amount
        );
    }

    public BankTransaction CreateWithdrawTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            location: _entityManager.GetEntityData(_entityManager.GetNetEntity(source)).Item2.EntityName,
            type: BankTransactionType.Withdraw,
            status: BankTransactionStatus.Success,
            balanceChangeType: BankBalanceChangeType.Expense,
            amount: amount
        );
    }

    public BankTransaction CreateBuyTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            location: _entityManager.GetEntityData(_entityManager.GetNetEntity(source)).Item2.EntityName,
            type: BankTransactionType.Buy,
            status: BankTransactionStatus.Success,
            balanceChangeType: BankBalanceChangeType.Expense,
            amount: amount
        );
    }

    public bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction)
    {
        var mind = _entSysManager.GetEntitySystem<SharedMindSystem>();
        if (!mind.TryGetMind(uid, out var mindId, out _))
            return false;

        var ev = new BankExecuteTransactionEvent(uid, netUid, transaction);
        _entityManager.EventBus.RaiseLocalEvent(mindId, ev);

        return !ev.Cancelled;
    }

    public bool TryGetBankAccount(EntityUid mobUid, [NotNullWhen(true)] out BankAccountComponent? bank)
    {
        bank = default;
        var mind = _entSysManager.GetEntitySystem<SharedMindSystem>();
        return mind.TryGetMind(mobUid, out var mindId, out _) && _entityManager.TryGetComponent(mindId, out bank);
    }
}
