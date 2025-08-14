using Content.Shared.Mobs;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.Zombies;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem
{
    private void InitPirates()
    {
        SubscribeLocalEvent<PirateComponent, MobStateChangedEvent>(OnPirateMobStateChanged);
        SubscribeLocalEvent<PirateComponent, EntityZombifiedEvent>(OnPirateZombified);
        SubscribeLocalEvent<PirateComponent, ComponentRemove>(OnPirateComponentRemoved);
    }

    private void OnPirateMobStateChanged(Entity<PirateComponent> entity, ref MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead && TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent))
        {
            CheckRoundShouldEnd((entity.Comp.PiratesProgress, progressComponent));
        }
    }

    private void OnPirateZombified(Entity<PirateComponent> entity, ref EntityZombifiedEvent args)
    {
        RemCompDeferred(entity, entity.Comp);
    }

    private void OnPirateComponentRemoved(Entity<PirateComponent> entity, ref ComponentRemove args)
    {
        if (TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent))
        {
            CheckRoundShouldEnd((entity.Comp.PiratesProgress, progressComponent));
        }
    }
}