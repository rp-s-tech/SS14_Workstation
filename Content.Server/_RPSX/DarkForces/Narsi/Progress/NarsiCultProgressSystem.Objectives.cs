﻿using System;
using Content.Server.RPSX.DarkForces.Narsi.Progress.Components;
using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.DarkForces.Narsi.Progress.Objectives;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.IoC;
using Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Summon;
using Content.Server.Mind;
using Robust.Shared.Log;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress;

public sealed partial class NarsiCultProgressSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string BuildingAltarObjective = "NarsiBuildingAltarObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string BuildingForgeObjective = "NarsiBuildingForgeObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string BuildingPilonObjective = "NarsiBuildingPilonObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string CreatureEggObjective = "NarsiCreatureEggObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string NarsiKillObjective = "NarsiKillObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string NarsiRitualObjective = "NarsiRitualObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string NarsiSummonObjective = "NarsiSummonObjective";
    [Dependency] private readonly MindSystem _mind = default!;
    private void InitializeObjectives()
    {
        SubscribeLocalEvent<NarsiCultObjectiveCompleted>(OnObjectiveCompleted);
    }

    /*private void UpdateObjectives()
    {
        if (_activeProgress is not { } progress)
            return;

        var objectives = progress.Comp.NarsiObjectives;
        if (objectives.NarsiSummonObjective != null)
            return;

        var curTime = _gameTiming.CurTime;
        if (objectives.NarsiObjectivesCheckTime > curTime)
            return;

        if (!IsObjectivesCompleted(progress))
        {
            objectives.NarsiObjectivesCheckTime = curTime + objectives.NarsiObjectivesCheckThreshold;
            return;
        }

        CreateNarsiSummonObjective(progress);
        objectives.NarsiObjectivesCheckTime = TimeSpan.Zero;
    }*/

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_activeProgress != null && _gameTiming.CurTime >= _activeProgress.Value.Comp.ObjectivesSpawnTime && _activeProgress.Value.Comp.NarsiObjectives.Objectives.Count == 0)
        {
            OnObjectivesInit(_activeProgress.Value);
            var cultists = EntityQueryEnumerator<NarsiCultistComponent>();
            while (cultists.MoveNext(out var uid, out _))
            {
                AddNarsiObjectives(uid);
            }
        }
    }

    private void OnObjectivesInit(Entity<NarsiCultProgressComponent> progress)
    {
        CreateObjective(progress, BuildingAltarObjective);
        CreateObjective(progress, BuildingForgeObjective);
        CreateObjective(progress, BuildingPilonObjective);
        CreateObjective(progress, CreatureEggObjective);

        var objectives = progress.Comp.NarsiObjectives;

        CreateSomeObjectives(progress, NarsiKillObjective, objectives.MinKills, objectives.MaxKills);

        CreateSomeObjectives(progress, NarsiRitualObjective, objectives.MinRituals, objectives.MaxRituals);

        objectives.NarsiObjectivesCheckTime = _gameTiming.CurTime + objectives.NarsiObjectivesCheckThreshold;
    }

    private EntityUid? CreateObjective(Entity<NarsiCultProgressComponent> objectiveProgress, string objectiveId)
    {
        var objective = _objectivesSystem.TryCreateGroupObjective(objectiveId);
        if (objective == null)
            return null;

        var narsiObjectives = objectiveProgress.Comp.NarsiObjectives.Objectives;
        narsiObjectives.Add(objective.Value);
        return objective;
    }

    private void CreateSomeObjectives(Entity<NarsiCultProgressComponent> objectiveProgress, string objectiveId, int min, int max)
    {
        var counter = 0;
        var objectivesCount = _random.Next(min, max);
        while (counter < objectivesCount)
        {
            counter++;
            CreateObjective(objectiveProgress, objectiveId);
        }
    }

    private void OnObjectiveCompleted(NarsiCultObjectiveCompleted args)
    {
        if (!TryComp<NarsiObjectiveComponent>(args.Objective, out var objective) || TryComp<NarsiCultSummonObjectiveComponent>(args.Objective, out _))
            return;

        if (objective.Completed)
            return;

        if (_activeProgress is not {} progress)
            return;

        var bloodPoints = objective.BloodScore;
        progress.Comp.BloodPoints += bloodPoints;

        SendMessageFromNarsi(progress, $"Вы выполнили задание. За это вы получили {bloodPoints} очков Крови. Используйте их для изучения новых способностей или улучшения текущих!");

        objective.Completed = true;

        if (IsObjectivesCompleted(progress))
        {
            CreateNarsiSummonObjective(progress);
        }
    }

    private bool IsObjectivesCompleted(Entity<NarsiCultProgressComponent> objectivesProgress)
    {
        if (objectivesProgress.Comp.NarsiObjectives.Objectives.Count == 0) return false;
        foreach (var objective in objectivesProgress.Comp.NarsiObjectives.Objectives)
        {
            var ev = new ObjectiveGetProgressEvent();
            RaiseLocalEvent(objective, ref ev);

            if (ev.Progress < 1f)
                return false;
        }

        return true;
    }

    private void CreateNarsiSummonObjective(Entity<NarsiCultProgressComponent> progress)
    {
        var summonObjective = CreateObjective(progress, NarsiSummonObjective);
        if (summonObjective == null)
            return;

        if (TryComp(summonObjective, out NarsiObjectiveComponent? comp))
            comp.Completed = false;

        progress.Comp.NarsiObjectives.CanSummonNarsi = true;
        progress.Comp.NarsiObjectives.NarsiSummonObjective = summonObjective;

        var cultists = EntityQueryEnumerator<NarsiCultistComponent>();
        while (cultists.MoveNext(out var uid, out _))
        {
            if (!_mindSystem.TryGetMind(uid, out var mindId, out var mindComp))
                continue;

            _mindSystem.AddObjective(mindId, mindComp, summonObjective.Value);
        }

        SendMessageFromNarsi(progress, "Наконец! Все готово, чтобы призвать меня");
    }

    private void AddNarsiObjectives(EntityUid uid)
    {
        if (_activeProgress == null)
            return;

        if (!_mindSystem.TryGetMind(uid, out var mindId, out var mindComp))
            return;

        foreach (var objective in _activeProgress.Value.Comp.NarsiObjectives.Objectives)
        {
            if (!mindComp.Objectives.Contains(objective))
                _mindSystem.AddObjective(mindId, mindComp, objective);
        }
    }

    public bool CanSummonNarsi()
    {
        if (_activeProgress is { Comp.NarsiObjectives.CanSummonNarsi: true })
            return true;

        return false;
    }
}
