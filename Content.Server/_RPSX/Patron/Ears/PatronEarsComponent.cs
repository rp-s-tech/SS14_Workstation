using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Patron.Ears;

[RegisterComponent]
public sealed partial class PatronEarsComponent : Component
{
    [DataField]
    public string Sprite = "Clothing/Head/Hats/catears.rsi";

    [DataField]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();
}
