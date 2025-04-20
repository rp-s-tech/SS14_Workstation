namespace Content.Client.SecretStation.Medical;

[RegisterComponent]
public sealed partial class MedicalDropperVisualsComponent : Component;

public enum MedicalDropperVisualLayers : byte
{
    Empty,
    WithBottle
}

public enum MedicalDropperBottleVisualLayers : byte
{
    Empty,
    Full,
    ThreeQuarters,
    Half,
    OneQuarters,
}
