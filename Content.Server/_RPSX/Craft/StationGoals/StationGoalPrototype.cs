using Content.Server.RPSX.Craft.StationGoals.Graph;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Craft.StationGoals;

[Prototype("stationGoal")]
public sealed class StationGoalPrototype : IPrototype
{
    [IdDataFieldAttribute]
    public string ID { get; } = default!;

    [DataField("canStartAutomatic", serverOnly: true)]
    public readonly bool CanStartAutomatic = true;

    [DataField("graph", serverOnly: true)]
    public readonly StationGoalGraph[] Graphs = default!;

    public int CurrentGraphIndex = 0;

    public void Cleanup()
    {
        CurrentGraphIndex = 0;
        foreach (var graph in Graphs)
        {
            graph.Cleanup();
        }
    }
}

