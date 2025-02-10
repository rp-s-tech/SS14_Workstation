// Exodus - Stamina Refactor
using System.Security.Cryptography;
using Content.Shared.Damage.Components;
using Content.Shared.Inventory;

namespace Content.Shared.Damage.Systems;

public abstract partial class SharedStaminaSystem
{
    private void OnRefreshDecay(EntityUid uid, StaminaComponent component, RefreshDecayEvent ev)
    {
        if (component.LastDamage + component.Cooldown > _timing.CurTime)
            return;

        ev.ModifyDecay(component.BaseDecay);
    }
    public void RefreshDecay(EntityUid uid, StaminaComponent? stamina = null)
    {
        if (!Resolve(uid, ref stamina, false))
            return;

        if (_timing.ApplyingState)
            return;

        var ev = new RefreshDecayEvent();
        RaiseLocalEvent(uid, ev);

        if (MathHelper.CloseTo(stamina.Decay, ev.Decay))
            return;

        stamina.Decay = ev.Decay;

        if (stamina.Decay < 0)
            EnsureComp<ActiveStaminaComponent>(uid);

        Dirty(uid, stamina);
    }
}

/// <summary>
///  Determine entity's stamina decay
///  If you want this event to be raised,
///  call <see cref="MovementSpeedModifierSystem.RefreshMovementSpeedModifiers"/>.
/// </summary>
public sealed class RefreshDecayEvent : EntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots { get; } = ~SlotFlags.POCKET;

    public float Decay { get; private set; } = 0.0f;

    public void ModifyDecay(float add)
    {
        Decay += add;
    }
}
