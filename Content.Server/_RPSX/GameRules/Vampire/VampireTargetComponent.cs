using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.GameRules.Vampire;

[RegisterComponent]
public sealed partial class VampireTargetComponent : Component
{
    [DataField("BloodDrinkedAmmount")]
    public int BloodDrinkedAmmount;
}
