using Content.Server.RPSX.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;

namespace Content.Server.RPSX.Damage.Systems;

public sealed class StaminaDamageArmorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StaminaArmorComponent, InventoryRelayedEvent<StaminaDamageModifyEvent>>(OnStaminaDamage);
    }

    private void OnStaminaDamage(EntityUid uid, StaminaArmorComponent component, InventoryRelayedEvent<StaminaDamageModifyEvent> args)
    {
        args.Args.Damage = component.Coefficient * args.Args.Damage;
    }
}
