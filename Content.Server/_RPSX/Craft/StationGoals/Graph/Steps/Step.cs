using Content.Server.RPSX.Craft.StationGoals.Graph.Steps;

namespace Content.Server.RPSX.Craft.StationGoals.Graph;

[ImplicitDataDefinitionForInheritors]
public abstract partial class Step
{
    [DataField("name", serverOnly: true)]
    private string _name = string.Empty;

    [DataField("delay", serverOnly: true)]
    public int Delay = 0;

    public string Name
    {
        get
        {
            if (String.IsNullOrEmpty(_name))
            {
                return this.GetType().Name;
            }

            return _name;
        }
    }

    internal Dictionary<StepDataKey, object> Results = new Dictionary<StepDataKey, object>();
    internal ExecuteState State = ExecuteState.Idle;

    internal void Execute(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        if (!CanStartStep())
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} cant start cause of state {State}");
            return;
        }

        if (IsDelayRequired())
        {
            State = ExecuteState.WaitingDelay;
            system.AskForDelay(Delay);
            system.Logger.RootSawmill.Debug($"Step: {Name} waiting for delay");
            return;
        }

        State = ExecuteStep(results, system);
    }

    private bool CanStartStep()
    {
        return State != ExecuteState.Interrupted && State != ExecuteState.Finished;
    }

    private bool IsDelayRequired()
    {
        return State != ExecuteState.WaitingDelay && Delay != 0;
    }

    internal abstract ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system);
    public virtual void Cleanup() { }
}
