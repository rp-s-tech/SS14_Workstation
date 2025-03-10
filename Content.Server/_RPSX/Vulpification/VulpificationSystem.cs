using Content.Server.Polymorph.Systems;
using Content.Shared.Humanoid;
using Robust.Shared.Random;
using Content.Shared.Hands.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server.RPSX.VulpificationVirus;
public sealed class VulpificationSystem : EntitySystem
{
    [Dependency] private readonly PolymorphSystem _poly = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VulpificationComponent, MeleeHitEvent>(OnMeleeHit);
    }
    private void OnMeleeHit(EntityUid uid, VulpificationComponent comp, MeleeHitEvent args)
    {
        if (_entityManager.TryGetComponent(uid, out HandsComponent? hands) && hands.ActiveHandEntity != null)
            return;

        foreach (var entity in args.HitEntities)
        {
            if (!_entityManager.TryGetComponent(entity, out HumanoidAppearanceComponent? humanoidAppearance) ||
                humanoidAppearance.Species == "Vulpkanin" ||
                _entityManager.HasComponent<VulpificationComponent>(entity))
                continue;

            if (_random.Prob(comp.SuccessChance))
            {
                _poly.PolymorphEntity(entity, comp.PolymorphPrototypeName);
            }

        }
    }
}
