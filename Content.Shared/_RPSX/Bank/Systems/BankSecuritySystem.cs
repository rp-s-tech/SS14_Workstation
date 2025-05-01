using Content.Shared.Cargo.Components;
using Content.Shared.RPSX.Bank.Components;
using Content.Shared.Examine;
using Content.Shared.Stacks;

namespace Content.Shared.RPSX.Bank.Systems;

public sealed partial class BankSecuritySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CashComponent, StackSplitEvent>(OnStackSplitEvent);
        SubscribeLocalEvent<BankSecureCashComponent, StacksMergedEvent>(OnStackMerged);
        SubscribeLocalEvent<BankSecureCashComponent, StacksMergeAttemptEvent>(OnStacksMergeAttempt);
        SubscribeLocalEvent<BankSecureCashComponent, StacksAddContainerAttempt>(OnStacksAddContainerAttempt);
        SubscribeLocalEvent<BankSecureCashComponent, ExaminedEvent>(OnExaminedEvent);
    }

    private void OnStacksAddContainerAttempt(EntityUid uid, BankSecureCashComponent component, StacksAddContainerAttempt args)
    {
        var donor = GetEntity(args.Donor);
        var recipient = GetEntity(args.Recipient);

        if (!CanStacksMerge(donor, recipient))
        {
            args.Cancel();
        }
    }

    private void OnStacksMergeAttempt(EntityUid uid, BankSecureCashComponent component, StacksMergeAttemptEvent args)
    {
        var donor = GetEntity(args.Donor);
        var recipient = GetEntity(args.Recipient);

        if (!CanStacksMerge(donor, recipient))
        {
            args.Cancel();
        }
    }

    private bool CanStacksMerge(EntityUid donor, EntityUid recipient)
    {
        if (HasComp<BankSecureCashComponent>(donor) && !HasComp<BankSecureCashComponent>(recipient))
            return false;

        if (HasComp<BankSecureCashComponent>(recipient) && !HasComp<BankSecureCashComponent>(donor))
            return false;

        return true;
    }

    private void OnExaminedEvent(EntityUid uid, BankSecureCashComponent component, ExaminedEvent args)
    {
        if (args.IsInDetailsRange)
        {
            args.PushMarkup(Loc.GetString("bank-secure-cash-markup"));
        }
    }

    private void OnStackMerged(EntityUid uid, BankSecureCashComponent component, StacksMergedEvent args)
    {
        if (HasComp<BankSecureCashComponent>(GetEntity(args.Donor)))
        {
            EnsureComp<BankSecureCashComponent>(GetEntity(args.Recipient));
        }
    }

    private void OnStackSplitEvent(EntityUid uid, CashComponent component, ref StackSplitEvent args)
    {
        if (!HasComp<BankSecureCashComponent>(uid))
        {
            RemComp<BankSecureCashComponent>(args.NewId);
        }
    }
}
