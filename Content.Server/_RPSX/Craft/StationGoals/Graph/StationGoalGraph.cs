using System.Linq;
using Content.Server.RPSX.Craft.StationGoals.Graph.Steps;
using Content.Server.RPSX.Utils;

namespace Content.Server.RPSX.Craft.StationGoals.Graph;

[Serializable]
[DataDefinition]
public sealed partial class StationGoalGraph
{
    [DataField("name", serverOnly: true, required: true)]
    public string Name = default!;

    [DataField("delay", serverOnly: true)]
    public int Delay = 0;

    [DataField("steps", serverOnly: true)]
    private Step[] _steps = Array.Empty<Step>();

    internal ExecuteState State = ExecuteState.Idle;

    internal Dictionary<StepDataKey, object> StepsResults = new Dictionary<StepDataKey, object>();

    internal int CurrentStepIndex = 0;

    public void Cleanup()
    {
        foreach (var step in _steps)
        {
            step.Cleanup();
            step.State = ExecuteState.Idle;
        }

        CurrentStepIndex = 0;
        State = ExecuteState.Idle;
    }

    public bool Execute(StationGoalPaperSystem system)
    {
        if (!IsStateValidToStart())
            return false;

        if (IsDelayRequired())
        {
            system.Logger.RootSawmill.Debug($"Graph: {Name} waiting delay");
            State = ExecuteState.WaitingDelay;
            system.AskForDelay(Delay);
            return false;
        }

        State = ExecuteState.InProgress;

        foreach (var (step, index) in _steps.WithIndex())
        {
            if (index < CurrentStepIndex)
                continue;

            CurrentStepIndex = index;
            step.Execute(StepsResults, system);
            if (!CanExecuteNextStep(step, system))
            {
                system.Logger.RootSawmill.Debug($"Graph: {Name} interrupted by {step}");
                State = ExecuteState.InnerInterrupted;

                return false;
            }

            StepsResults = StepsResults
                        .Concat(step.Results)
                        .ToDictionary(x => x.Key, x => x.Value);
        }

        system.Logger.RootSawmill.Debug($"Graph: {Name} finished success");
        State = ExecuteState.Finished;
        return true;
    }

    private bool CanExecuteNextStep(Step step, StationGoalPaperSystem system)
    {
        return step.State == ExecuteState.Finished;
    }

    private bool IsStateValidToStart()
    {
        return State != ExecuteState.Interrupted || State != ExecuteState.Finished;
    }

    private bool IsDelayRequired()
    {
        return State != ExecuteState.WaitingDelay && Delay != 0;
    }

    public Step? TryGetStepByName(string name)
    {
        return _steps.First(step => step.Name.Equals(name));
    }
}
