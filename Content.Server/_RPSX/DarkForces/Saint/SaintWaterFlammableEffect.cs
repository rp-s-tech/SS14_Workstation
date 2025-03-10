using Content.Server.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Log;

namespace Content.Server.RPSX.DarkForces.Saint.Reagent;

public sealed partial class SaintWaterFlammableEffect : EntityEffect
{
    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs)
        {
            return;
        }
        var entityManager = args.EntityManager;

        var saintWaterDrinkEvent = new OnSaintWaterFlammableEvent(reagentArgs.TargetEntity, reagentArgs.Quantity, reagentArgs.Method);
        entityManager.EventBus.RaiseLocalEvent(reagentArgs.TargetEntity, saintWaterDrinkEvent);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "Помогает бороться с нечистью";
    }
}
