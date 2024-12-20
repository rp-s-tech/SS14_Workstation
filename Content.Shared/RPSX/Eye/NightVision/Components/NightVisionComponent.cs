using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.RPSX.Eye.NightVision.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class NightVisionComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public bool IsNightVisionOn;

    [DataField]
    [AutoNetworkedField]
    public Color Color = Color.Green;
}

public sealed partial class NVInstantActionEvent : InstantActionEvent;

public sealed partial class NVGInstanActionEvent : InstantActionEvent
{
    [DataField]
    public SoundSpecifier OnOffSound = new SoundPathSpecifier("/Audio/DarkStation/Misc/nvg_switch.ogg");
}
