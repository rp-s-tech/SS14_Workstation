using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Server.Polymorph.Systems;
using System.Linq;
using Content.Server.Actions;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar;

public sealed partial class NarsiAltarSystem
{
    [Dependency] private readonly ActionsSystem _actionsSystem = default!;
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
        polymorphComponent.AltarEntityUid = uid;
        polymorphComponent.ReturnToAltar = args.ReturnToAltar;

        EntityUid? actionId = null;
        _actionsSystem.AddAction(args.Target, ref actionId, "ActionRevertPolymorph", args.Target);
    }

    private void OnPolymorpgReverted(EntityUid uid, NarsiPolymorphComponent component, PolymorphRevertedEvent args)
    {
        var altar = component.AltarEntityUid;
        if (!component.ReturnToAltar || !EntityManager.EntityExists(altar))
            return;

        var transform = Transform(altar);
        _transform.SetCoordinates(args.Original, transform.Coordinates);

        var action = _actionsSystem.GetActions(uid).ToList().Where(c => MetaData(c.Id).EntityPrototype?.ID == "ActionRevertPolymorph").First().Id;
        _actionsSystem.RemoveAction(action);
    }
}
