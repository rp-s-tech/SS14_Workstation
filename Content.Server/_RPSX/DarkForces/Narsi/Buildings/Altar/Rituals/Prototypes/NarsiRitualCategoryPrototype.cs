using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;

[Prototype, Serializable]
public sealed class NarsiRitualCategoryPrototype : IPrototype
{
    [IdDataFieldAttribute]
    public string ID { get; } = default!;

    [DataField(required: true, serverOnly: true)]
    public readonly string Name = default!;

    [DataField(required: true, serverOnly: true)]
    public List<ProtoId<NarsiRitualPrototype>> Rituals = new();
}
