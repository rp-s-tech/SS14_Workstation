using Robust.Shared.Serialization;
using Content.Shared.RPSX.Bank.Transactions;

namespace Content.Shared.RPSX.Bank.Components;

[RegisterComponent]
public sealed partial class BankAccountComponent : Component
{
    [DataField]
    public int Balance = 0;

    [ViewVariables(VVAccess.ReadOnly)]
    public List<BankCredit> BankCredits;

    [ViewVariables(VVAccess.ReadOnly)]
    public List<BankDeposit> BankDeposits;

    [ViewVariables]
    public List<BankTransaction> BankTransactions = new();
}

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class BankCredit
{
    [DataField]
    public int Percent { get; set; }
    [DataField]
    public int Summ { get; set; }
    [DataField]
    public int CreditStart { get; set; } // Начало депозита
    [DataField]
    public int NextPayment { get; set; } // В сменах
    [DataField]
    public int CreditEnding { get; set; } // Сколько смен пройдет с начала депозита
}

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class BankDeposit
{
    [DataField]
    public int Percent { get; set; }
    [DataField]
    public int Summ { get; set; }
    [DataField]
    public int DepositStart { get; set; } // Начало депозита
    [DataField]
    public int NextPayment { get; set; } // В сменах
    [DataField]
    public int DepositEnding { get; set; } // Сколько смен пройдет с начала депозита
}
