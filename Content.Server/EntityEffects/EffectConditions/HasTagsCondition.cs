using System.Linq;
using Content.Shared.EntityEffects;
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.EntityEffects.EffectConditions;

[UsedImplicitly]
public sealed partial class HasTags : EntityEffectCondition
{
    [DataField(customTypeSerializer: typeof(PrototypeIdHashSetSerializer<TagPrototype>))]
    public HashSet<string> Tags = default!;

    [DataField]
    public bool All = false;

    [DataField]
    public bool Invert = false;

    public override bool Condition(EntityEffectBaseArgs args)
    {
        if (args.EntityManager.TryGetComponent<TagComponent>(args.TargetEntity, out var tagComp))
        {
            if (All)
            {
                var allTagsPresent = tagComp.Tags.All(tag => Tags.Contains(tag));
                return allTagsPresent ^ Invert;
            }
            else
            {
                var anyTagPresent = tagComp.Tags.Any(tag => Tags.Contains(tag));
                return anyTagPresent ^ Invert;
            }
        }

        return false;
    }

    public override string GuidebookExplanation(IPrototypeManager prototype)
    {
        // this should somehow be made (much) nicer.
        return Loc.GetString("reagent-effect-condition-guidebook-has-tag", ("tag", string.Join(", ", Tags)), ("invert", Invert));
    }
}
