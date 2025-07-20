using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.GameRules.Pirates;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public abstract partial class BasePirateComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid PiratesProgress;
}