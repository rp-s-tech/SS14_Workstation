using Content.Shared.GameTicking;
using Content.Shared.Preferences;
using Content.Shared.RPSX.Bank.Components;
using Content.Shared.RPSX.Bank.Events;
using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.Bank.Systems;

public sealed partial class BankSystem
{
    private void InitializeAccount()
    {
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);
        SubscribeLocalEvent<BankAccountComponent, BankExecuteTransactionEvent>(OnBankAccountChanged);
    }

    private async void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        if (!_mindSystem.TryGetMind(args.Mob, out var mindId, out var mind))
            return;
        EnsureComp<BankAccountComponent>(mindId, out var bankAccount);
        RefreshBankBalanceAsync(mindId);
    }

    private async void OnBankAccountChanged(EntityUid mindId, BankAccountComponent bank, BankExecuteTransactionEvent args)
    {
        RefreshBankBalanceAsync(mindId);
        var transaction = args.Transaction;
        if (transaction.Amount <= 0)
        {
            args.Cancel();
            transaction.Status = BankTransactionStatus.Failure;
            bank.BankTransactions.Add(transaction);
            return;
        }

        if (transaction.BalanceChangeType == BankBalanceChangeType.Expense && bank.Balance < transaction.Amount)
        {
            args.Cancel();
            transaction.Status = BankTransactionStatus.Failure;
            bank.BankTransactions.Add(transaction);
            return;
        }

        if (!_prefsManager.TryGetCachedPreferences(args.UserId, out var prefs))
            return;
        var character = prefs.SelectedCharacter;
        var index = prefs.IndexOfCharacter(character);

        if (character is not HumanoidCharacterProfile)
            return;

        bank.Balance = GetBalanceByTransaction(bank, transaction);
        UpdateProfile(args.UserId, bank, index);
        bank.BankTransactions.Add(args.Transaction);
        Dirty(mindId, bank);
    }

    private int GetBalanceByTransaction(BankAccountComponent bank, BankTransaction transaction)
    {
        if (transaction.BalanceChangeType == BankBalanceChangeType.Expense)
            return bank.Balance - transaction.Amount;
        return bank.Balance + transaction.Amount;
    }
}
