using System.Linq;
using Content.Shared.EntityEffects;
using Content.Shared.Mobs.Components;
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.EntityEffects.EffectConditions;

[UsedImplicitly]
public sealed partial class IsMob : EntityEffectCondition
{
    [DataField]
    public bool Invert = false;

    public override bool Condition(EntityEffectBaseArgs args)
    {
        return args.EntityManager.HasComponent<MobStateComponent>(args.TargetEntity) ^ Invert;
    }

    public override string GuidebookExplanation(IPrototypeManager prototype)
    {
        // this should somehow be made (much) nicer.
        return Loc.GetString("reagent-effect-condition-guidebook-is-mob", ("invert", Invert));
    }
}
