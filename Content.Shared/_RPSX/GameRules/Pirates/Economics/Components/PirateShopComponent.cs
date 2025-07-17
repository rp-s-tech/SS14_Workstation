using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.RPSX.Shared.GameRules.Pirates.Economics;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class PirateShopComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public TimeSpan NextTick = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public TimeSpan NextTickDelay = TimeSpan.FromMilliseconds(1300);

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public List<(EntProtoId, int)> ProductsQueue = new();

    [DataField]
    public SoundSpecifier ErrorSound = new SoundCollectionSpecifier("CargoError");

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextDenySoundTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan DenySoundDelay = TimeSpan.FromSeconds(2);

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public EntityUid ProgressEntity;
}
