namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Common;

public sealed partial class CleanupStep : Step
{
    [DataField("step", serverOnly: true, required: true)]
    private string _stepName = default!;
    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var goal = system.CurrentGoal;
        if (goal == null)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interrupted goal is null");
            return ExecuteState.Interrupted;
        }

        var currentGraph = goal.Graphs[goal.CurrentGraphIndex];
        var targetStep = currentGraph.TryGetStepByName(_stepName);

        targetStep?.Cleanup();

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }
}
