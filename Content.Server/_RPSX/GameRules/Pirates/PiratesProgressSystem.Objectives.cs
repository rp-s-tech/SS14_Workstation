using System.Linq;
using Content.Server.RPSX.GameRules.Pirates.Objectives;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Objectives;
using Robust.Shared.Prototypes;
using Content.Shared.Objectives.Components;
using Content.Server.Mind;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem
{
    [Dependency] private readonly ObjectivesSystem _objectivesSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    private void SpawnObjectives(Entity<PiratesProgressComponent> progress)
    {
        foreach (var (key, value) in progress.Comp.ObjectivesToSpawn)
        {
            for (var i = 0; i < value; i++)
            {
                CreateObjective(progress, key);
            }
        }
    }

    private void CheckObjectives(Entity<PiratesProgressComponent> progress)
    {
        var progressComp = progress.Comp;

        if (progressComp.ObjectivesCheckTime > _timing.CurTime) return;
        progressComp.ObjectivesCheckTime += progressComp.ObjectivesCheckThreshold;

        foreach (var objective in progressComp.Objectives)
        {
            if (!TryComp<PirateObjectiveComponent>(objective, out var objectiveComponent))
            {
                Log.Fatal("Smb is fucking idiot and added pirate objective without component");
                return;
            }

            IsObjectiveCompleted((objective, objectiveComponent));
        }
        Dirty(progress);
    }

    private double IsObjectiveCompleted(Entity<PirateObjectiveComponent> objective)
    {
        var ev = new ObjectiveGetProgressEvent();
        RaiseLocalEvent(objective, ref ev);

        if (!objective.Comp.RewardGiven)
        {
            _economicsSystem.ChangePiratesBalance(objective.Comp.PiratesProgress, objective.Comp.Reward);
            objective.Comp.RewardGiven = true;
            SendMessageFromHead(objective.Comp.PiratesProgress, $"objective completed {MetaData(objective).EntityName}");
        }
        if (ev.Progress is not float objProgress)
            return 0;

        return objProgress;
    }

    private void CreateObjective(Entity<PiratesProgressComponent> objectiveProgress, EntProtoId objectiveId)
    {
        var objective = _objectivesSystem.TryCreateGroupObjective(objectiveId);
        if (objective == null || !TryComp(objective, out PirateObjectiveComponent? objectiveComponent))
            return;

        var objectives = objectiveProgress.Comp.Objectives;
        objectives.Add(objective.Value);

        Dirty(objectiveProgress);

        foreach (var pirate in objectiveProgress.Comp.StartedPirates)
        {
            if (!_mindSystem.TryGetMind(pirate, out var mindId, out var mind))
                return;

            _mindSystem.AddObjective(mindId, mind, objective.Value);
        }
    }

    private (double, double) GetObjectivesProgress(Entity<PiratesProgressComponent> progressEntity)
    {
        var progress = (0d, 0d);
        foreach (var objective in progressEntity.Comp.Objectives)
        {
            if (!TryComp<PirateObjectiveComponent>(objective, out var objectiveComponent))
            {
                Log.Fatal("Smb is fucking idiot and added pirate objective without component");
                return (0f, 0f);
            }

            var prog = IsObjectiveCompleted((objective, objectiveComponent));

            switch (objectiveComponent.Priority)
            {
                case "additional":
                    progress.Item2 += prog;
                    break;
                case "main":
                    progress.Item1 += prog;
                    break;
            }
        }

        progress = (Math.Round(progress.Item1, 2), Math.Round(progress.Item2, 2));
        return progress;
    }
}
