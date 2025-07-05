using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared.RPSX.GameRules.Pirates.EntityEffects;

[DataDefinition]
public sealed partial class DrinkAlcoholEffect : EventEntityEffect<DrinkAlcoholEffect>
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "";
    }
}
