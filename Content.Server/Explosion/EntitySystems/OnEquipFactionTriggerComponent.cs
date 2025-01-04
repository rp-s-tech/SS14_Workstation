using Content.Shared.NPC.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Explosion.Components;

[RegisterComponent]
public sealed partial class OnEquipFactionTriggerComponent : Component
{
    [DataField]
    public ProtoId<NpcFactionPrototype> Faction;

    [DataField]
    public float Delay = 5f;

    [DataField]
    public SoundSpecifier? BeepSound;

    [DataField]
    public float? InitialBeepDelay;

    [DataField]
    public float BeepInterval = 1;

    [DataField]
    public bool StopOnUnequip = true;
}
