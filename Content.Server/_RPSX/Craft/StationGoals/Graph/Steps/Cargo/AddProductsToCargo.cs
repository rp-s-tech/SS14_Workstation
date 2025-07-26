using System.Linq;
using Content.Server.RPSX.Bridges;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Common;

public sealed partial class AddProductsToCargo : Step
{
    [DataField("advancedCargoPrototypes", serverOnly: true, required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    public List<string> AdvancedCargoPrototypes { get; set; } = new();

    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var cargoBridge = entityManager.System<CargoBridge>();

        var prototypes = prototypeManager.EnumeratePrototypes<CargoProductPrototype>();
        var filteredPrototypes = prototypes
            .ToList()
            .FindAll(prototype => AdvancedCargoPrototypes.Contains(prototype.ID) && !prototype.Enabled);

        cargoBridge.AddAdvancedPrototypes(filteredPrototypes);

        return ExecuteState.Finished;
    }
}
