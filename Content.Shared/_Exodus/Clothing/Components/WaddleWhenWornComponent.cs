// Based on https://github.com/space-exodus/space-station-14/blob/e0174a7cb79c080db69a12c0c36121830df30f9d/Content.Shared/Clothing/Components/WaddleWhenWornComponent.cs
using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Clothing.Components;

/// <summary>
/// Defines something as causing waddling when worn.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WaddleWhenWornComponent : Component
{
    ///<summary>
    /// How high should they hop during the waddle? Higher hop = more energy.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Vector2 HopIntensity = new(0, 0.25f);

    /// <summary>
    /// How far should they rock backward and forward during the waddle?
    /// Each step will alternate between this being a positive and negative rotation. More rock = more scary.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float TumbleIntensity = 20.0f;

    /// <summary>
    /// How long should a complete step take? Less time = more chaos.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float AnimationLength = 0.66f;

    /// <summary>
    /// How much shorter should the animation be when running?
    /// </summary>
    [DataField, AutoNetworkedField]
    public float RunAnimationLengthMultiplier = 0.568f;
}
