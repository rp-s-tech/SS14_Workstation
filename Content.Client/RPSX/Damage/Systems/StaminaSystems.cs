using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Robust.Client.Player;
using Robust.Shared.Player;


namespace Content.Client.Damage.Systems;

public sealed partial class StaminaSystem : SharedStaminaSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public event EventHandler? SyncStamina;

    public StaminaUIState State
    {
        get
        {
            var ent = _playerManager.LocalEntity;
            if (ent == null || !TryComp<StaminaComponent>(ent, out var component))
                return new StaminaUIState(visible: false);
            return new StaminaUIState(MathF.Max(0, (component.CritThreshold - component.StaminaDamage) / component.CritThreshold));
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StaminaComponent, LocalPlayerAttachedEvent>(OnAttached);
        SubscribeLocalEvent<StaminaComponent, LocalPlayerDetachedEvent>(OnDetached);
    }

    private void OnAttached(EntityUid uid, StaminaComponent component, LocalPlayerAttachedEvent args)
    {
        if (_playerManager.LocalEntity == uid)
            UpdateHud();
    }
    private void OnDetached(EntityUid uid, StaminaComponent component, LocalPlayerDetachedEvent args)
    {
        if (_playerManager.LocalEntity == uid)
            UpdateHud();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateHud();
    }

    private void UpdateHud()
    {
        SyncStamina?.Invoke(this, new());
    }
}


[Serializable]
public struct StaminaUIState
{
    public bool Visible;
    public float Value;
    public StaminaUIState(float value = 1.0f, bool visible = true)
    {
        Visible = visible;
        Value = value;
    }
}
