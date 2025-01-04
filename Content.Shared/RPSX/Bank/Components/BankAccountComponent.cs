namespace Content.Shared.RPSX.Bank.Components;

[RegisterComponent]
public sealed partial class BankAccountComponent : Component
{
    [DataField]
    public int Balance;
}
