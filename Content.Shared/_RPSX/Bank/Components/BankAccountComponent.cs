using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.Bank.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BankAccountComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadOnly)]
    public int Balance;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadOnly)]
    public List<BankTransaction> BankTransactions = new();
}
