using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.Bank.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BankAccountComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public int Balance;

    [ViewVariables, AutoNetworkedField]
    public List<BankTransaction> BankTransactions = new();
}
