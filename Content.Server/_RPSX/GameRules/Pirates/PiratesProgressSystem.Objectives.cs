using Content.Server.RPSX.GameRules.Pirates.Objectives;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Objectives;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem
{
    [Dependency] private readonly ObjectivesSystem _objectivesSystem = default!;

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
            var ev = new CheckObjectiveEvent();
            RaiseLocalEvent(objective, ref ev);
            if (!ev.Completed) return;
            if (!TryComp<PirateObjectiveComponent>(objective, out var comp) || comp.RewardGiven) return;
            _economicsSystem.ChangePiratesBalance(progress, comp.Reward);
            comp.RewardGiven = true;
        }
        progressComp.AreObjectivesCompleted = true;
        // Здесь надо идти чекать короче по поводу того насколько выиграли/проиграли пираты
        Dirty(progress);
    }

    private void CreateObjective(Entity<PiratesProgressComponent> objectiveProgress, EntProtoId objectiveId)
    {
        var objective = _objectivesSystem.TryCreateGroupObjective(objectiveId);
        if (objective == null || !TryComp(objective, out PirateObjectiveComponent? objectiveComponent))
            return;

        objectiveComponent.PiratesProgress = objectiveProgress;
        var narsiObjectives = objectiveProgress.Comp.Objectives;
        narsiObjectives.Add((objective.Value));

        Dirty(objectiveProgress);
    }
}

[ByRefEvent]
public record struct CheckObjectiveEvent(bool Completed = false);
