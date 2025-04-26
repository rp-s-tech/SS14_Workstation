using System;
using System.Linq;
using Content.Shared.Database;
using Content.Shared.RPSX.Bank.Transactions;
using Content.Shared.RPSX.Roles.Salary;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Player;
using System.Collections.Generic;
using CrewMemberStationSalaryComponent = Content.Server.RPSX.Roles.Salary.Components.CrewMemberStationSalaryComponent;

namespace Content.Server.RPSX.Roles.Salary.Systems;

public sealed partial class CrewMemberSalarySystem
{
    private void StartStationPrePayDay(Entity<CrewMemberStationSalaryComponent> salaries)
    {
        var station = salaries.Owner;
        try
        {
            var stationRecords = _cachedEntries[station];
            foreach (var record in stationRecords.Values)
            {
                if (record.NetUserId is not { } userId)
                    continue;

                if (record.MobEntity is not { } netEntity)
                    continue;

                if (record.Salary is not { } salary)
                    continue;

                var uid = GetEntity(netEntity);
                if (!CrewMemberHaveSalary(uid) || salary.PrePaidCount >= 2)
                    continue;

                var prepaid = (int) Math.Round(salary.Salary * 0.15);
                salary.PrePaid += prepaid;
                salary.PrePaidCount += 1;

                var transaction = _bankManager.CreateSalaryTransaction(prepaid, BankSalarySource.CentralCommand);
                _bankManager.TryExecuteTransaction(uid, userId, transaction);
                MakePayDayNotify(station);
            }
        }
        catch (KeyNotFoundException)
        {
            return;
        }
    }

    private void StartStationPayDay(EntityUid station)
    {
        foreach (var record in _cachedEntries[station].Values)
        {
            if (record.NetUserId is not { } userId || record.MobEntity is not { } netEntity ||
                record.Salary is not { } salary)
            {
                continue;
            }

            var uid = GetEntity(netEntity);
            if (!CrewMemberHaveSalary(uid))
            {
                continue;
            }

            var crewSalary = CalculateCrewMemberSalary(salary);
            if (crewSalary == 0)
            {
                continue;
            }

            if (_mobStateSystem.IsDead(uid))
            {
                var salaryCount = (int) Math.Round(crewSalary * 0.30);
                var partialTransaction = _bankManager.CreateSalaryTransaction(salaryCount, BankSalarySource.CentralCommand);
                _bankManager.TryExecuteTransaction(uid, userId, partialTransaction);
                _adminLogger.Add(LogType.Salary, LogImpact.Medium, $"Added salary to user ${record.NetUserId}; Salary - {salaryCount}");
                continue;
            }
            var transaction = _bankManager.CreateSalaryTransaction(crewSalary, BankSalarySource.CentralCommand);
            _bankManager.TryExecuteTransaction(uid, userId, transaction);

            _adminLogger.Add(LogType.Salary, LogImpact.Medium, $"Added salary to user ${record.NetUserId}; Salary - {crewSalary}");
            MakePayDayNotify(station);
        }
    }

    private int CalculateCrewMemberSalary(CrewSalaryEntry entry)
    {
        var bonuses = entry.SalaryBonuses.Sum(bonus => bonus.Bonus);
        var penalties = entry.SalaryPenalties.Sum(penalty => penalty.Penalty);

        return entry.Salary - entry.PrePaid + bonuses - penalties;
    }

    private bool CrewMemberHaveSalary(EntityUid uid)
    {
        return _mindSystem.TryGetMind(uid, out var mindId, out _) && !_roleSystem.MindIsAntagonist(mindId);
    }


    public void UpdateCrewMemberBonus(EntityUid station, EntityUid changer, int bonus, uint recordKey)
    {
        var stationRecordKey = _sharedStationRecords.Convert((GetNetEntity(station), recordKey));

        if (!_recordsSystem.TryGetRecord<GeneralStationRecord>(stationRecordKey, out var record))
            return;

        if (record.Salary is not { } salary)
        {
            _recordsSystem.Synchronize(station);
            return;
        }

        if ((float) bonus / salary.Salary > 0.5f)
        {
            _recordsSystem.Synchronize(station);
            return;
        }

        salary.SalaryBonuses.Add(new CrewSalaryBonus(bonus, ""));
        _recordsSystem.Synchronize(station);

        _adminLogger.Add(LogType.Salary, LogImpact.Medium,
            $"Added salary bonus to user ${record.NetUserId}; bonus - {bonus} by {changer}");
    }

    public void UpdateCrewMemberPenalty(EntityUid station, EntityUid changer, int penalty, uint recordKey)
    {
        var stationRecordKey = _sharedStationRecords.Convert((GetNetEntity(station), recordKey));
        if (!_recordsSystem.TryGetRecord<GeneralStationRecord>(stationRecordKey, out var record))
            return;

        record.Salary?.SalaryPenalties.Add(new CrewSalaryPenalty(penalty, ""));
        _recordsSystem.Synchronize(station);
        _adminLogger.Add(LogType.Salary, LogImpact.Medium,
            $"Added penalty to user ${record.NetUserId}; penalty - {penalty} by {changer}");
    }
}
