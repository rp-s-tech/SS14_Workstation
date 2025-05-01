using System;
using System.Collections.Generic;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.Bank.Transactions;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.RPSX.Roles.Salary.Systems;

public sealed partial class CrewMemberSalarySystem
{
    private int _antagBaseSalary;
    private int _antagMaxSalary;

    private void InitializeAntags()
    {
        _cfg.OnValueChanged(Content.Shared.RPSX.CCVars.RPSXCCVars.EconomyAntagBaseSalary, OnAntagBaseSalaryChanged);
        _cfg.OnValueChanged(Content.Shared.RPSX.CCVars.RPSXCCVars.EconomyAntagMaxSalary, OnAntagMaxSalaryChanged);

        _antagBaseSalary = _cfg.GetCVar(Content.Shared.RPSX.CCVars.RPSXCCVars.EconomyAntagBaseSalary);
        _antagMaxSalary = _cfg.GetCVar(Content.Shared.RPSX.CCVars.RPSXCCVars.EconomyAntagMaxSalary);
    }

    private void OnAntagMaxSalaryChanged(int newMaxSalary)
    {
        _antagMaxSalary = newMaxSalary;
    }

    private void OnAntagBaseSalaryChanged(int newBaseSalary)
    {
        _antagBaseSalary = newBaseSalary;
    }

    private void OnRoundEndedAntags()
    {
        var query = EntityQueryEnumerator<ActorComponent>();
        while (query.MoveNext(out var uid, out var actorComponent))
        {
            if (_mobStateSystem.IsDead(uid))
                continue;

            if (!_mindSystem.TryGetMind(uid, out var mindId, out var mind))
                continue;

            HandleAntagObjectives(uid, mindId, mind, actorComponent.PlayerSession.UserId, mind.Objectives);
        }
    }

    private void HandleAntagObjectives(EntityUid uid, EntityUid mindId, MindComponent mind, NetUserId userId, List<EntityUid> objectives)
    {
        var totalSum = 0;
        foreach (var objective in objectives)
        {
            var ev = new ObjectiveGetProgressEvent(mindId, mind);
            RaiseLocalEvent(objective, ref ev);

            if (ev.Progress is < 1f)
                continue;

            if (!TryComp<ObjectiveComponent>(objective, out var objectiveComponent))
                continue;

            var antagSalary = (int) Math.Round(_antagBaseSalary * objectiveComponent.Difficulty);
            totalSum += antagSalary;
        }

        if (_antagMaxSalary != -1 && totalSum > _antagMaxSalary)
            totalSum = _antagMaxSalary;

        var transaction = _bankManager.CreateSalaryTransaction(totalSum, BankSalarySource.Unknown);
        _bankManager.TryExecuteTransaction(uid, userId, transaction);
    }
}
