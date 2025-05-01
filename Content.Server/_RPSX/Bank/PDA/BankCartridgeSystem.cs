using Content.Shared.RPSX.Bank.PDA.Components;
using Content.Shared.CartridgeLoader;
using Content.Shared.RPSX.Bank.PDA;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Server.CartridgeLoader;
using Content.Shared.RPSX.Bank.Systems;

namespace Content.Server.RPSX.Bank.Systems.PDA
{
    public sealed class BankCartridgeSystem : EntitySystem
    {
        [Dependency] private readonly BankSystem _bankSystem = default!;
        [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoaderSystem = default!;
        [Dependency] private readonly IBankManager _bankManager = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<BankCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
            SubscribeLocalEvent<BankCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
        }

        private void OnUiReady(EntityUid uid, BankCartridgeComponent component, CartridgeUiReadyEvent args)
        {
            UpdateUiState(uid, args.Loader, component);
        }

        private void OnUiMessage(EntityUid uid, BankCartridgeComponent component, CartridgeMessageEvent args)
        {
            UpdateUiState(uid, GetEntity(args.LoaderUid), component);
        }

        private void UpdateUiState(EntityUid uid, EntityUid loaderUid, BankCartridgeComponent component)
        {
            if (!_bankSystem.TryGetGeneralStationRecordAndStation(loaderUid, out var record, out _) || record.MobEntity == null)
                return;

            var idCardUser = GetEntity(record.MobEntity.Value);

            if (!_bankManager.TryGetBankAccount(idCardUser, out var bank, out _))
                return;

            var userName = MetaData(idCardUser).EntityName;
            var transactionsList = bank.BankTransactions;
            var state = new BankCartridgeUiState(userName, bank.Balance, transactionsList);

            _cartridgeLoaderSystem.UpdateCartridgeUiState(loaderUid, state);
        }
    }
}
