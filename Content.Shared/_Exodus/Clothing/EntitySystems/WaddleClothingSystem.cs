// Based on https://github.com/space-exodus/space-station-14/blob/e0174a7cb79c080db69a12c0c36121830df30f9d/Content.Shared/Clothing/EntitySystems/WaddleClothingSystem.cs
using Content.Shared.Clothing;
using Content.Shared.Exodus.Clothing.Components;
using Content.Shared.Exodus.Movement.Components;

namespace Content.Shared.Exodus.Clothing.EntitySystems;

public sealed class WaddleClothingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WaddleWhenWornComponent, ClothingGotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<WaddleWhenWornComponent, ClothingGotUnequippedEvent>(OnGotUnequipped);
    }

    private void OnGotEquipped(EntityUid entity, WaddleWhenWornComponent comp, ClothingGotEquippedEvent args)
    {
        var waddleAnimComp = EnsureComp<WaddleAnimationComponent>(args.Wearer);

        waddleAnimComp.AnimationLength = comp.AnimationLength;
        waddleAnimComp.HopIntensity = comp.HopIntensity;
        waddleAnimComp.RunAnimationLengthMultiplier = comp.RunAnimationLengthMultiplier;
        waddleAnimComp.TumbleIntensity = comp.TumbleIntensity;
    }

    private void OnGotUnequipped(EntityUid entity, WaddleWhenWornComponent comp, ClothingGotUnequippedEvent args)
    {
        RemComp<WaddleAnimationComponent>(args.Wearer);
    }
}