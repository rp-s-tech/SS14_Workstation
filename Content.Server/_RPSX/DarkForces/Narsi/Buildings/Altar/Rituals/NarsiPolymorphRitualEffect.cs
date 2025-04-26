﻿using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Shared.Polymorph;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;

[DataDefinition]
public sealed partial class NarsiPolymorphRitualEffect : NarsiRitualEffect
{
    [DataField(required: true)]
    public PolymorphConfiguration Configuration = default!;

    [DataField]
    public bool ReturnToAltar;

    [DataField]
    public bool DeleteBuckled;

    public override void MakeRitualEffect(EntityUid altar, EntityUid perfomer, NarsiAltarComponent component, IEntityManager entityManager)
    {
        var polymorphPerformerEvent = new NarsiRequestPolymorphEvent(
            perfomer,
            Configuration,
            ReturnToAltar
        );
        entityManager.EventBus.RaiseLocalEvent(altar, polymorphPerformerEvent);

        if (!DeleteBuckled)
            return;

        entityManager.QueueDeleteEntity(component.BuckledEntity);
        component.BuckledEntity = null;
    }
}
