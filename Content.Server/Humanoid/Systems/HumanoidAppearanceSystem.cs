using Content.Shared.Examine; // ADT-Changeling-Tweak
using System.Linq; // ADT-Changeling-Tweak
using Content.Shared.Decals; // ADT-Changeling-Tweak
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.IdentityManagement; // ADT-Changeling-Tweak
using Content.Shared.Preferences;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.Prototypes; // ADT-Changeling-Tweak

namespace Content.Server.Humanoid;

public sealed partial class HumanoidAppearanceSystem : SharedHumanoidAppearanceSystem
{
    [Dependency] private readonly MarkingManager _markingManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!; // ADT-Changeling-Tweak

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HumanoidAppearanceComponent, HumanoidMarkingModifierMarkingSetMessage>(OnMarkingsSet);
        SubscribeLocalEvent<HumanoidAppearanceComponent, HumanoidMarkingModifierBaseLayersSetMessage>(OnBaseLayersSet);
        SubscribeLocalEvent<HumanoidAppearanceComponent, GetVerbsEvent<Verb>>(OnVerbsRequest);
        //SubscribeLocalEvent<HumanoidAppearanceComponent, ExaminedEvent>(OnExamined);
    }

    /// <summary>
    ///     Removes a marking from a humanoid by ID.
    /// </summary>
    /// <param name="uid">Humanoid mob's UID</param>
    /// <param name="marking">The marking to try and remove.</param>
    /// <param name="sync">Whether to immediately sync this to the humanoid</param>
    /// <param name="humanoid">Humanoid component of the entity</param>
    public void RemoveMarking(EntityUid uid, string marking, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
    {
        if (!Resolve(uid, ref humanoid)
            || !_markingManager.Markings.TryGetValue(marking, out var prototype))
        {
            return;
        }

        humanoid.MarkingSet.Remove(prototype.MarkingCategory, marking);

        if (sync)
            Dirty(uid, humanoid);
    }

    /// <summary>
    ///     Removes a marking from a humanoid by category and index.
    /// </summary>
    /// <param name="uid">Humanoid mob's UID</param>
    /// <param name="category">Category of the marking</param>
    /// <param name="index">Index of the marking</param>
    /// <param name="humanoid">Humanoid component of the entity</param>
    public void RemoveMarking(EntityUid uid, MarkingCategories category, int index, HumanoidAppearanceComponent? humanoid = null)
    {
        if (index < 0
            || !Resolve(uid, ref humanoid)
            || !humanoid.MarkingSet.TryGetCategory(category, out var markings)
            || index >= markings.Count)
        {
            return;
        }

        humanoid.MarkingSet.Remove(category, index);
        Dirty(uid, humanoid);
    }

    /// <summary>
    ///     Sets the marking ID of the humanoid in a category at an index in the category's list.
    /// </summary>
    /// <param name="uid">Humanoid mob's UID</param>
    /// <param name="category">Category of the marking</param>
    /// <param name="index">Index of the marking</param>
    /// <param name="markingId">The marking ID to use</param>
    /// <param name="humanoid">Humanoid component of the entity</param>
    public void SetMarkingId(EntityUid uid, MarkingCategories category, int index, string markingId, HumanoidAppearanceComponent? humanoid = null)
    {
        if (index < 0
            || !_markingManager.MarkingsByCategory(category).TryGetValue(markingId, out var markingPrototype)
            || !Resolve(uid, ref humanoid)
            || !humanoid.MarkingSet.TryGetCategory(category, out var markings)
            || index >= markings.Count)
        {
            return;
        }

        var marking = markingPrototype.AsMarking();
        for (var i = 0; i < marking.MarkingColors.Count && i < markings[index].MarkingColors.Count; i++)
        {
            marking.SetColor(i, markings[index].MarkingColors[i]);
        }

        humanoid.MarkingSet.Replace(category, index, marking);
        Dirty(uid, humanoid);
    }

    /// <summary>
    ///     Sets the marking colors of the humanoid in a category at an index in the category's list.
    /// </summary>
    /// <param name="uid">Humanoid mob's UID</param>
    /// <param name="category">Category of the marking</param>
    /// <param name="index">Index of the marking</param>
    /// <param name="colors">The marking colors to use</param>
    /// <param name="humanoid">Humanoid component of the entity</param>
    public void SetMarkingColor(EntityUid uid, MarkingCategories category, int index, List<Color> colors,
        HumanoidAppearanceComponent? humanoid = null)
    {
        if (index < 0
            || !Resolve(uid, ref humanoid)
            || !humanoid.MarkingSet.TryGetCategory(category, out var markings)
            || index >= markings.Count)
        {
            return;
        }

        for (var i = 0; i < markings[index].MarkingColors.Count && i < colors.Count; i++)
        {
            markings[index].SetColor(i, colors[i]);
        }

        Dirty(uid, humanoid);
    }

    // ADT-Changeling-Tweak-Start
    /// <summary>
    /// Takes ID of the species prototype, returns UI-friendly name of the species.
    /// </summary>
    public string GetSpeciesRepresentation(string speciesId)
    {
        if (_prototypeManager.TryIndex<SpeciesPrototype>(speciesId, out var species))
        {
            return Loc.GetString(species.Name);
        }
        else
        {
            return Loc.GetString("humanoid-appearance-component-unknown-species");
        }
    }

    public string GetAgeRepresentation(string species, int age)
    {
        _prototypeManager.TryIndex<SpeciesPrototype>(species, out var speciesPrototype);

        if (speciesPrototype == null)
        {
            Logger.Error("Tried to get age representation of species that couldn't be indexed: " + species);
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.YoungAge)
        {
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.OldAge)
        {
            return Loc.GetString("identity-age-middle-aged");
        }

        return Loc.GetString("identity-age-old");
    }
    // ADT-Changeling-Tweak-End
}
