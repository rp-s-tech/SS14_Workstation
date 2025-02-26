using Content.Server.Fluids.EntitySystems;
using Content.Server.Nutrition.Components;
using Robust.Shared.Physics.Events;
using Content.Server.RPSX.EntityEffects.Effects;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.EntityEffects;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.RPSX.RandomTeleport;

public sealed class RandomFoodTeleportSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutions = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RandomFoodTeleportComponent, StartCollideEvent>(OnStartCollide);
        SubscribeLocalEvent<RandomFoodTeleportComponent, LandEvent>(OnLand);
    }

    private void OnStartCollide(Entity<RandomFoodTeleportComponent> ent, ref StartCollideEvent args)
    {
        TryTeleport(ent, args.OtherEntity);
    }

    private void OnLand(Entity<RandomFoodTeleportComponent> ent, ref LandEvent args)
    {
        var coordinates = Transform(ent).Coordinates;
        _audio.PlayPvs(_audio.GetSound(ent.Comp.SoundCollection), coordinates, AudioParams.Default.WithVariation(0.125f));

        if (EntityManager.TryGetComponent(ent, out FoodComponent? foodComp) &&
            EntityManager.TryGetComponent(ent, out SolutionContainerManagerComponent? solMgr))
        {
            if (_solutions.TryGetSolution(solMgr, foodComp.Solution, out var solution))
            {
                _puddle.TrySpillAt(ent, solution, out _, false);
            }
        }
        EntityManager.QueueDeleteEntity(ent);
    }

    private void TryTeleport(Entity<RandomFoodTeleportComponent> ent, EntityUid target)
    {
        if (ent.Comp.Teleported || Transform(target).Anchored)
            return;

        ent.Comp.Teleported = true;

        var teleportEffect = new RandomTeleportEffect
        {
            Radius = ent.Comp.TeleportRadius,
            Sound = ent.Comp.TeleportSound
        };

        teleportEffect.Effect(new EntityEffectBaseArgs(target, EntityManager));
    }
}
