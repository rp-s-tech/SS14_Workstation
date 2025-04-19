using Content.Client.Damage.Systems;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Timing;


namespace Content.Client.RPSX.Damage.UI;

public sealed class StaminaUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<StaminaSystem>
{
    [UISystemDependency] private readonly StaminaSystem? _staminaSystem = default!;
    private StaminaUI? UI => UIManager.GetActiveUIWidgetOrNull<StaminaUI>();

    public override void Initialize()
    {
        base.Initialize();
        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
        gameplayStateLoad.OnScreenUnload += OnScreenUnload;
    }
    private void OnScreenUnload()
    {
        SyncStamina();
    }
    private void OnScreenLoad()
    {
        SyncStamina();
    }
    public void OnSystemLoaded(StaminaSystem system)
    {
        system.SyncStamina += SystemOnSyncStamina;
    }
    public void OnSystemUnloaded(StaminaSystem system)
    {
        system.SyncStamina -= SystemOnSyncStamina;
    }

    private void SystemOnSyncStamina(object? sender, EventArgs a) =>
        SyncStamina();

    public void OnStateEntered(GameplayState state)
    {
        SyncStamina();
    }

    private void SyncStamina()
    {
        if (_staminaSystem == null || UI == null)
            return;
        var state = _staminaSystem.State;
        UI.Visible = state.Visible;
        UI.SetValue(state.Value);
    }
}
