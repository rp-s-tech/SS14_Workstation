using Content.Server.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Saint.Reagent;

public sealed partial class SaintWaterDrinkEffect : EntityEffect
{
    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs)
            return;

        var entityManager = args.EntityManager;

        var saintWaterDrinkEvent = new OnSaintWaterDrinkEvent(reagentArgs.TargetEntity, reagentArgs.Quantity);
        entityManager.EventBus.RaiseLocalEvent(reagentArgs.TargetEntity, saintWaterDrinkEvent);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "Помогает бороться с нечистью";
    }
}
