using Content.RPSX.Server.GameRules.Pirates.Objectives;
using Content.RPSX.Shared.GameRules.Pirates;
using Content.RPSX.Shared.GameRules.Pirates.Economics;
using Content.Server.Objectives;
using Robust.Shared.Prototypes;

namespace Content.RPSX.Server.GameRules.Pirates;

public sealed partial class PiratesProgressSystem
{
    public EntProtoId StealCommandStaffObjective = "StealCmdObjective";
    public EntProtoId EarnMoneyObjective = "BalanceIncreasingObjective";
    public EntProtoId StealItemsObjective = "StealItemsObjective";
    public EntProtoId StealCriticalItemsObjective = "StealCriticalItemsObjective";

    [Dependency] private readonly ObjectivesSystem _objectivesSystem = default!;
    [Dependency] private readonly PirateEconomicsSystem _economicsSystem = default!;

    private void SpawnObjectives(Entity<PiratesProgressComponent> progress)
    {
        CreateObjective(progress, StealCommandStaffObjective);
        CreateObjective(progress, StealCommandStaffObjective);
        CreateObjective(progress, EarnMoneyObjective);
        if (progress.Comp.GamePlay == PiratesGamePlay.Silent)
        {
            CreateObjective(progress, StealItemsObjective);
            CreateObjective(progress, StealItemsObjective);
            CreateObjective(progress, StealItemsObjective);
            CreateObjective(progress, StealItemsObjective);
        }
        else
        {
            CreateObjective(progress, StealCriticalItemsObjective);
            CreateObjective(progress, StealCriticalItemsObjective);
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
