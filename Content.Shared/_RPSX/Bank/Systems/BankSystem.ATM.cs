using Content.Shared.RPSX.Bank.Components;
using Content.Shared.Database;
using Content.Shared.RPSX.Bank;
using Content.Shared.RPSX.Bank.BUI;
using Content.Shared.RPSX.Bank.Events;
using Content.Shared.Stacks;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.Audio.Systems;
using Content.Shared.Popups;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Administration.Logs;

namespace Content.Shared.RPSX.Bank.Systems;

public sealed partial class BankATMSystem : EntitySystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string CashType = "SpaceCash";

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IBankManager _bankManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BankATMComponent, BankWithdrawMessage>(OnWithdraw);
        SubscribeLocalEvent<BankATMComponent, BankDepositMessage>(OnDeposit);
        SubscribeLocalEvent<BankATMComponent, BoundUIOpenedEvent>(OnATMUIOpen);
        SubscribeLocalEvent<BankATMComponent, EntInsertedIntoContainerMessage>(OnCashSlotChanged);
        SubscribeLocalEvent<BankATMComponent, EntRemovedFromContainerMessage>(OnCashSlotChanged);
    }

    private void OnWithdraw(EntityUid uid, BankATMComponent component, BankWithdrawMessage args)
    {
        if (!TryComp<ActorComponent>(args.Actor, out var actor))
            return;

        // Устанавливаем баланс в 0 для временного скрытия
        if (_uiSystem.HasUi(uid, args.UiKey))
        {
            _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(0, true, 0));
        }

        if (!_bankManager.TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        GetInsertedCashAmount(uid, out var deposit);

        if (bank.Balance < args.Amount)
        {
            PlayDenySound(uid, component, "bank-insufficient-funds");
            return;
        }

        // Попытка снять средства
        var transaction = _bankManager.CreateWithdrawTransaction(uid, args.Amount);
        if (!_bankManager.TryExecuteTransaction(args.Actor, actor.PlayerSession.UserId, transaction))
        {
            PlayDenySound(uid, component, "bank-atm-menu-transaction-denied");
            return;
        }

        _adminLogger.Add(LogType.ATMUsage, LogImpact.Low,
            $"{ToPrettyString(args.Actor):actor} withdrew {args.Amount} from {ToPrettyString(uid)}");
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, deposit));

        RaiseLocalEvent(uid, new SpawnCashEvent(CashType, GetNetEntity(uid), args.Amount));
        PlayConfirmSound(uid, component, "bank-atm-menu-withdraw-successful");
    }

    private void OnDeposit(EntityUid uid, BankATMComponent component, BankDepositMessage args)
    {
        if (!TryComp<ActorComponent>(args.Actor, out var actor))
            return;

        // Устанавливаем баланс в 0 для временного скрытия
        if (_uiSystem.HasUi(uid, args.UiKey))
        {
            _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(0, true, 0));
        }

        if (!_bankManager.TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        GetInsertedCashAmount(uid, out var deposit);

        var transaction = _bankManager.CreateDepositTransaction(uid, deposit);
        if (!_bankManager.TryExecuteTransaction(args.Actor, actor.PlayerSession.UserId, transaction))
        {
            PlayDenySound(uid, component, "bank-atm-menu-transaction-denied");
            return;
        }

        _adminLogger.Add(LogType.ATMUsage, LogImpact.Low,
            $"{ToPrettyString(args.Actor):actor} deposited {deposit} into {ToPrettyString(uid)}");
        _containerSystem.CleanContainer(_containerSystem.GetContainer(uid, BankATMComponent.CashSlotSlotId));
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, 0));

        PlayConfirmSound(uid, component, "bank-atm-menu-deposit-successful");
    }

    private void OnCashSlotChanged(EntityUid uid, BankATMComponent component, ContainerModifiedMessage args)
    {
        var actor = _uiSystem.GetActors(uid, BankATMMenuUiKey.ATM).FirstOrNull();
        if (actor == null)
            return;

        // Устанавливаем баланс в 0 для временного скрытия
        _uiSystem.SetUiState(uid, BankATMMenuUiKey.ATM, new BankATMMenuInterfaceState(0, true, 0));

        GetInsertedCashAmount(uid, out var deposit);

        if (!_bankManager.TryGetBankAccount(actor.Value, out var bank, out _))
            return;

        _uiSystem.SetUiState(uid, BankATMMenuUiKey.ATM, new BankATMMenuInterfaceState(bank.Balance, true, deposit));
    }

    private void OnATMUIOpen(EntityUid uid, BankATMComponent component, BoundUIOpenedEvent args)
    {
        // Устанавливаем баланс в 0 для временного скрытия
        if (_uiSystem.HasUi(uid, args.UiKey))
        {
            _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(0, true, 0));
        }

        if (!_bankManager.TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        GetInsertedCashAmount(uid, out var deposit);
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, deposit));
    }

    private void GetInsertedCashAmount(EntityUid uid, out int amount)
    {
        if (!_itemSlots.TryGetSlot(uid, BankATMComponent.CashSlotSlotId, out var slot) ||
            !TryComp<StackComponent>(slot.Item, out var cashStack))
        {
            amount = 0;
            return;
        }

        amount = cashStack.Count;
    }

    private void PlayDenySound(EntityUid uid, BankATMComponent component, string message)
    {
        _audio.PlayPvs(_audio.ResolveSound(component.ErrorSound), uid);
        _popup.PopupEntity(Robust.Shared.Localization.Loc.GetString(message), uid, uid);
    }

    private void PlayConfirmSound(EntityUid uid, BankATMComponent component, string message)
    {
        _audio.PlayPvs(_audio.ResolveSound(component.ConfirmSound), uid);
        _popup.PopupEntity(Robust.Shared.Localization.Loc.GetString(message), uid, uid);
    }
}

[Serializable, NetSerializable]
public sealed class SpawnCashEvent : EntityEventArgs
{
    /// <summary>
    ///     Cash type prototype
    /// </summary>
    public string CashType;

    /// <summary>
    ///     Entity, on which we spawn cash
    /// </summary>
    public NetEntity Where;

    /// <summary>
    ///     Amount of cash
    /// </summary>
    public int Amount;

    public SpawnCashEvent(string cashType, NetEntity where, int amount)
    {
        CashType = cashType;
        Where = where;
        Amount = amount;
    }
}
