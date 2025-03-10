using Content.Shared.RPSX.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;

namespace Content.Shared.RPSX.Damage.Systems;

public sealed class StaminaDamageArmorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StaminaArmorComponent, StaminaDamageModifyEvent>(OnStaminaDamage);
        SubscribeLocalEvent<StaminaArmorComponent, InventoryRelayedEvent<StaminaDamageModifyEvent>>(OnParentStaminaDamage);

    }

    private void OnParentStaminaDamage(Entity<StaminaArmorComponent> entity, ref InventoryRelayedEvent<StaminaDamageModifyEvent> args)
    {
        args.Args.Damage = entity.Comp.Coefficient * args.Args.Damage;
    }

    private void OnStaminaDamage(Entity<StaminaArmorComponent> entity, ref StaminaDamageModifyEvent args)
    {
        args.Damage = entity.Comp.Coefficient * args.Damage;
    }
}
