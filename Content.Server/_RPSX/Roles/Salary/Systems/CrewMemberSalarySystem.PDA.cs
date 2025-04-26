using Content.Server.PDA.Ringer;
using Content.Shared.RPSX.Bank.PDA.Components;
using Content.Shared.CartridgeLoader;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.Roles.Salary.Systems;

public sealed partial class CrewMemberSalarySystem
{
    private void MakePayDayNotify(EntityUid station)
    {
        var stationTransform = Transform(station);
        var query = EntityQueryEnumerator<CartridgeLoaderComponent, RingerComponent, ContainerManagerComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var ringer, out var cont, out var transform))
        {
            if (stationTransform.MapID != transform.MapID)
                continue;

            if (!_cartridgeLoader.TryGetProgram<BankCartridgeComponent>(uid, out _, out _, false, comp,
                    cont))
                continue;

            _ringerSystem.RingerPlayRingtone((uid, ringer));
        }
    }
}
