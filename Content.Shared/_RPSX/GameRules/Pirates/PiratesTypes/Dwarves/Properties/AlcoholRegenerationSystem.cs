using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.Mobs.Systems;
using Content.Shared.RPSX.GameRules.Pirates.EntityEffects;

namespace Content.RPSX.Shared.GameRules.Pirates.PiratesTypes.Dwarves.Properties;

public sealed class AlcoholRegenerationSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ExecuteEntityEffectEvent<DrinkAlcoholEffect>>(OnDrinkAlcohol);
    }

    private void OnDrinkAlcohol(ref ExecuteEntityEffectEvent<DrinkAlcoholEffect> eventData)
    {
        var entity = eventData.Args.TargetEntity;

        if (!TryComp<DamageableComponent>(entity, out var damageable) || _mobState.IsDead(entity)
            || !TryComp<PirateDwarfComponent>(entity, out var healingComp))
            return;

        _damageable.TryChangeDamage(entity, healingComp.HealingDamage, true, false, damageable);
    }
}
