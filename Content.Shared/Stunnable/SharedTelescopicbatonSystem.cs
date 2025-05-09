using Content.Server.Stunnable.Components;
using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared.Stunnable;

public abstract class SharedTelescopicbatonSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TelescopicbatonComponent, GetMeleeDamageEvent>(OnGetMeleeDamage);
    }

    private void OnGetMeleeDamage(EntityUid uid, TelescopicbatonComponent component, ref GetMeleeDamageEvent args)
    {
        if (!component.Activated)
            return;

        // Don't apply damage if it's activated; just do stamina damage.
        args.Damage = new DamageSpecifier();
    }
}
