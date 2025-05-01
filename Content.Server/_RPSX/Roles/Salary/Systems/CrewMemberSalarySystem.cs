using Content.Server.Administration.Logs;
using Content.Server.CartridgeLoader;
using Content.Server.Mind;
using Content.Server.PDA.Ringer;
using Content.Server.StationRecords.Systems;
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles;
using Content.Shared.RPSX.Bank.Systems;
using Content.Shared.StationRecords;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.RPSX.Roles.Salary.Systems;

public sealed partial class CrewMemberSalarySystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedRoleSystem _roleSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly StationRecordsSystem _recordsSystem = default!;
    [Dependency] private readonly SharedStationRecordsSystem _sharedStationRecords = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoader = default!;
    [Dependency] private readonly RingerSystem _ringerSystem = default!;
    [Dependency] private readonly IBankManager _bankManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeRecords();
        InitializeStation();
        InitializeAntags();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateStationSalary();
    }
}
