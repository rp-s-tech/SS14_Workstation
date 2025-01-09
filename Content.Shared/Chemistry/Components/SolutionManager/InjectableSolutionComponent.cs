using Content.Shared.Whitelist; // Exodus-ThickSyringes

namespace Content.Shared.Chemistry.Components.SolutionManager;

/// <summary>
///     Denotes a solution which can be added with syringes.
/// </summary>
[RegisterComponent]
public sealed partial class InjectableSolutionComponent : Component
{

    /// <summary>
    /// Solution name which can be added with syringes.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Solution = "default";

    // Exodus-ThickSyringes-Start
    /// <summary>
    /// When not null this entity could be injected only by specified entities.
    /// It only works when injection performs an entity through interactions.
    /// </summary>
    [DataField]
    public EntityWhitelist? Whitelist = null;
    // Exodus-ThickSyringes-End
}
