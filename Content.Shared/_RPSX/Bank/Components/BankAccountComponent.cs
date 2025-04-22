using Content.Shared.RPSX.Bank.Transactions;

namespace Content.Shared.RPSX.Bank.Components;

[RegisterComponent]
public sealed partial class BankAccountComponent : Component
{
    [DataField]
    public int Balance;

    [ViewVariables]
    public List<BankTransaction> BankTransactions = new();
}
