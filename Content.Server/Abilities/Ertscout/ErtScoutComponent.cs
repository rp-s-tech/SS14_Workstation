namespace Content.Server.Abilities.ErtScout;

[RegisterComponent]
public sealed partial class ErtScoutComponent : Component
{
    [DataField]
    public EntityUid? Suit;
}
