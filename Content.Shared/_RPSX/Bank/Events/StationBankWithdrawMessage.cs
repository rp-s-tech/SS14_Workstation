﻿using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Bank.Events;

[Serializable, NetSerializable]

public sealed class StationBankWithdrawMessage : BoundUserInterfaceMessage
{
    //amount to withdraw. validation is happening server side but we still need client input from a text field.
    public int Amount;
    public string? Reason;
    public string? Description;
    public StationBankWithdrawMessage(int amount, string? reason, string? description)
    {
        Amount = amount;
        Reason = reason;
        Description = description;
    }
}
