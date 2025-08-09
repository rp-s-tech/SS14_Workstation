using System.Linq;

namespace Content.Shared.RPSX.GameRules.Pirates;

public sealed partial class BasePirateSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BasePirateComponent, MapInitEvent>(OnBaseMapInit);
    }

    private void OnBaseMapInit(Entity<BasePirateComponent> entity, ref MapInitEvent args)
    {
        var progress = EntityQuery<PiratesProgressComponent>()
            .Where(p => p.PiratesShuttle == Transform(entity).GridUid)
            .FirstOrDefault();
        if (progress == null)
        {
            PredictedQueueDel(entity.Owner);
            return;
        }
        entity.Comp.PiratesProgress = progress.CompOwner;
        entity.Comp.CompOwner = entity.Owner;

        if (HasComp<PirateComponent>(entity.Owner))
        {
            progress.StartedPirates.Add(entity.Owner);
            DirtyField(progress.CompOwner, progress, nameof(PiratesProgressComponent.StartedPirates));
        }
    }
}