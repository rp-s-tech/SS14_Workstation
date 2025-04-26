using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.RPSX.DarkForces.Narsi.Runes.Events;

public sealed partial class NarsiRuneInstantSpawnEvent : InstantActionEvent
{
    [DataField("prototype", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Prototype = default!;
}
