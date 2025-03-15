using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.Cargo;
using Content.Server.Cargo.Components;
using Content.Shared.Cargo.Components;
using Robust.Shared.Audio;

namespace Content.Server.Cargo.Systems
{
    public sealed partial class CargoSystem
    {
        public CargoOrderData PublicGetOrderData(CargoConsoleAddOrderMessage args, CargoProductPrototype cargoProduct, int id)
        {
            return GetOrderData(args, cargoProduct, id);
        }
        public bool PublicTryAddOrder(EntityUid dbUid, CargoOrderData data, StationCargoOrderDatabaseComponent component)
        {
            return TryAddOrder(dbUid, data, component);
        }
        public void PublicPlayDenySound(EntityUid uid)
        {
            _audio.PlayPvs(_audio.ResolveSound(new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_sigh.ogg")), uid);
        }
    }
}
