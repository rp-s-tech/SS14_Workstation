
using Robust.Shared.Audio;

namespace Content.Server.RPSX.RandomTeleport;

[RegisterComponent]
public sealed partial class RandomFoodTeleportComponent : Component
{
    [DataField]
    public float TeleportRadius = 5.0f;

    [DataField]
    public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    [DataField]
    public SoundSpecifier SoundCollection = new SoundCollectionSpecifier("desecration");


    [DataField]
    public bool Teleported = false;
}
