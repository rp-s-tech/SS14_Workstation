using System.Linq;
using Content.Server.RPSX.Craft.StationGoals.Graph;
using Content.Server.RPSX.Utils;
using Content.Shared.GameTicking;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Server.GameTicking.Events;

namespace Content.Server.RPSX.Craft.StationGoals;

public sealed class StationGoalPaperSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] internal readonly ILogManager Logger = default!;

    private TimeSpan _nextConditionCheck = TimeSpan.Zero;
    private bool _stationGoalDisabled;

    internal StationGoalPrototype? CurrentGoal;

    public void AskForDelay(int seconds)
    {
        _nextConditionCheck = _gameTiming.CurTime + new TimeSpan(hours: 0, minutes: 0, seconds: seconds);
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStarting);
        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
    }

    private void OnRoundStarting(RoundStartingEvent ev)
    {
        CleanupGoal();

        if (_stationGoalDisabled)
            return;

        SendRandomGoal();
    }

    private void OnRoundEnded(RoundEndedEvent ev)
    {
        _stationGoalDisabled = false;
        CleanupGoal();
    }

    public void DisableStationGoal()
    {
        _stationGoalDisabled = true;
    }

    private void SendRandomGoal()
    {
        var availableGoals = _prototypeManager.EnumeratePrototypes<StationGoalPrototype>()
            .Where(prototype => prototype.CanStartAutomatic)
            .ToList();

        var goal = _random.Pick(availableGoals);
        SendStationGoal(goal);
    }


    public void SendStationGoal(StationGoalPrototype goal)
    {
        CleanupGoal();

        CurrentGoal = goal;
        ExecuteGraphs();
    }

    private void CleanupGoal()
    {
        if (CurrentGoal == null)
            return;

        CurrentGoal.Cleanup();
        CurrentGoal = null;
    }

    private void ExecuteGraphs()
    {
        if (CurrentGoal == null)
            return;

        foreach (var (graph, index) in CurrentGoal.Graphs.WithIndex())
        {
            if (index < CurrentGoal.CurrentGraphIndex)
                continue;

            CurrentGoal.CurrentGraphIndex = index;

            if (!graph.Execute(this) && graph.State == ExecuteState.InnerInterrupted)
            {
                break;
            }
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (CurrentGoal == null || _nextConditionCheck == TimeSpan.Zero)
            return;

        if (_nextConditionCheck >= _gameTiming.CurTime)
            return;

        _nextConditionCheck = TimeSpan.Zero;
        ExecuteGraphs();
    }
}
