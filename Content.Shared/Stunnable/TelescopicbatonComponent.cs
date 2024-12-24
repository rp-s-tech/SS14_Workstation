using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Server.Stunnable.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedTelescopicbatonSystem))]
public sealed partial class TelescopicbatonComponent : Component
{
    [DataField("activated"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Activated = false;

    [DataField("sparksSound")]
    public SoundSpecifier SparksSound = new SoundCollectionSpecifier("sparks");
}
