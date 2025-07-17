using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.RPSX.Shared.GameRules.Pirates.PiratesTypes.Dwarves;

[RegisterComponent, NetworkedComponent]
public sealed partial class PirateDwarfComponent : Component
{
    [DataField]
    public DamageSpecifier HealingDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Blunt", -3},
            {"Slash", -3},
            {"Piercing", -3},
            {"Heat", -3},
            {"Cold", -3},
            {"Shock", -3},
            {"Burn", -3},
        }
    };
}
