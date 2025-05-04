using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Server.Polymorph.Systems;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar;

public sealed partial class NarsiAltarSystem
{
    private void InitPolymorph()
    {
        SubscribeLocalEvent<NarsiAltarComponent, NarsiRequestPolymorphEvent>(OnPolymorphRequest);
        SubscribeLocalEvent<NarsiPolymorphComponent, PolymorphRevertedEvent>(OnPolymorpgReverted);
    }

    private void OnPolymorphRequest(EntityUid uid, NarsiAltarComponent component, NarsiRequestPolymorphEvent args)
    {
        var polymorphEntity = _polymorph.PolymorphEntity(args.Target, args.Configuration);
        if (polymorphEntity == null)
            return;

        var polymorphComponent = EnsureComp<NarsiPolymorphComponent>(polymorphEntity.Value);
        _action.AddAction(polymorphEntity.Value, "ActionRevertPolymorph");
        polymorphComponent.AltarEntityUid = uid;
        polymorphComponent.ReturnToAltar = args.ReturnToAltar;
    }

    private void OnPolymorpgReverted(EntityUid uid, NarsiPolymorphComponent component, PolymorphRevertedEvent args)
    {
        var altar = component.AltarEntityUid;
        if (!component.ReturnToAltar || !EntityManager.EntityExists(altar))
            return;

        var transform = Transform(altar);
        _transform.SetCoordinates(args.Original, transform.Coordinates);
    }
}
