using Content.Server.RPSX.Bridges;
using Content.Shared.EntityEffects;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.GameRules.Vampire;

public sealed partial class VampireThirst : EntityEffect
{
    private const float DefaultHydrationFactor = 3.0f;

    [DataField("factor")]
    public float HydrationFactor { get; set; } = DefaultHydrationFactor;

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs)
            return;

        var target = reagentArgs.TargetEntity;
        var isVampire = IoCManager.Resolve<IVampireBridge>().IsVampire(target);
        if (!isVampire || !args.EntityManager.TryGetComponent(target, out ThirstComponent? thirst))
            return;

        args.EntityManager.System<ThirstSystem>().ModifyThirst(target, thirst, HydrationFactor);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return null;
    }
}
