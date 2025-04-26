using Content.Server.Database;
using Content.Server.Mind;
using Content.Server.Preferences.Managers;
using Content.Server.StationRecords.Systems;
using Content.Shared.RPSX.Bank.Components;
using Robust.Shared.Network;
using Content.Shared.StationRecords;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.PDA;
using Content.Shared.RPSX.Bank.Systems;
using Content.Shared.Mind;
using Content.Server.Stack;
using Content.Shared.Coordinates;

namespace Content.Server.RPSX.Bank.Systems
{
    public sealed partial class BankSystem : EntitySystem
    {
        [Dependency] private readonly StationRecordsSystem _stationRecordsSystem = default!;
        [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
        [Dependency] private readonly IServerDbManager _dbManager = default!;
        [Dependency] private readonly MindSystem _mindSystem = default!;
        [Dependency] private readonly StackSystem _stackSystem = default!;

        private ISawmill _log = default!;

        public override void Initialize()
        {
            base.Initialize();
            _log = Logger.GetSawmill("bank");
            InitializeAccount();

            SubscribeLocalEvent<BankATMComponent, SpawnCashEvent>(SpawnCash);
        }

        /// <summary>
        /// Асинхронно получает баланс из БД и обновляет BankAccountComponent у игрока,
        /// чтобы всегда иметь актуальные данные.
        /// </summary>
        public async void RefreshBankBalanceAsync(Entity<MindComponent?> mind)
        {
            // Пытаемся найти mind
            if (!Resolve(mind, ref mind.Comp))
                return;

            // Проверяем UserId
            if (mind.Comp.UserId == null)
                return;

            var userId = mind.Comp.UserId.Value;

            // Достаём из префов нужный слот персонажа
            if (!_prefsManager.TryGetCachedPreferences(userId, out var prefs))
                return;

            var character = prefs.SelectedCharacter;
            var index = prefs.IndexOfCharacter(character);

            if (character is not Content.Shared.Preferences.HumanoidCharacterProfile)
                return;

            if (!TryComp<BankAccountComponent>(mind, out var bank))
                return;

            // Считываем актуальное значение баланса из БД
            var account = await _dbManager.GetProfileEconomics(userId, index);
            if (account == null) return;
            // Устанавливаем баланс
            bank.Balance = account.Balance;
            Dirty(mind, bank);
            return;
        }
        /// <summary>
        /// Асинхронно обновляет экономику игрока в базе данных.
        /// </summary>
        public async void UpdateProfile(NetUserId userId, BankAccountComponent bank, int index)
        {
            await _dbManager.SaveProfileEconomics(userId, index, bank);
        }

        public bool TryGetGeneralStationRecordAndStation(EntityUid pda, [NotNullWhen(true)] out GeneralStationRecord? record,
            [NotNullWhen(true)] out EntityUid? station)
        {
            record = null;
            station = null;

            if (!TryComp<PdaComponent>(pda, out var comp) || comp.ContainedId == null)
                return false;

            if (!TryComp<StationRecordKeyStorageComponent>(comp.ContainedId, out var keyStorage) || keyStorage.Key == null)
                return false;

            station = keyStorage.Key.Value.OriginStation;

            if (!_stationRecordsSystem.TryGetRecord(keyStorage.Key.Value, out record))
                return false;

            _stationRecordsSystem.Synchronize(station.Value);
            return true;
        }

        public void SpawnCash(EntityUid uid, BankATMComponent bankATM, SpawnCashEvent args)
        {
            var stacks = _stackSystem.SpawnMultiple(args.CashType, args.Amount, GetEntity(args.Where).ToCoordinates());
            foreach (var stack in stacks)
            {
                RemComp<BankSecureCashComponent>(stack);
            }
        }

    }
}
