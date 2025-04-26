using Content.Server.Station.Events;
using Content.Shared.GameTicking;
using Content.Shared.RPSX.Roles.Salary;
using Content.Shared.StationRecords;
using Robust.Shared.Log;
using Robust.Shared.GameObjects;
using CrewMemberStationSalaryComponent = Content.Server.RPSX.Roles.Salary.Components.CrewMemberStationSalaryComponent;


namespace Content.Server.RPSX.Roles.Salary.Systems;

public sealed partial class CrewMemberSalarySystem
{
    private void InitializeStation()
    {
        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
        SubscribeLocalEvent<CrewMemberStationSalaryComponent, StationPostInitEvent>(OnStationPostInitEvent);
    }

    private void OnRoundEnded(RoundEndedEvent ev)
    {
        foreach (var station in _cachedEntries.Keys)
        {
            StartStationPayDay(station);
        }

        OnRoundEndedAntags();
    }

    private void LoadSalaries(Entity<CrewMemberStationSalaryComponent> stationSalary)
    {
        if (!_prototypeManager.TryIndex(stationSalary.Comp.Salaries, out var salaries))
            return;

        foreach (var salary in salaries.Salaries)
        {
            stationSalary.Comp.CachedSalaries[salary.Key.Id] = salary.Value;
        }
    }

    private void OnStationPostInitEvent(EntityUid uid, CrewMemberStationSalaryComponent component,
        ref StationPostInitEvent args)
    {
        LoadSalaries((uid, component));
        SetNextStationSalaryPeriod((uid, component));
    }

    private void UpdateStationSalary()
    {
        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<CrewMemberStationSalaryComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.PayDayTime > curTime)
                continue;

            StartStationPrePayDay((uid, component));
            SetNextStationSalaryPeriod((uid, component));
        }
    }

    private void SetNextStationSalaryPeriod(Entity<CrewMemberStationSalaryComponent> stationSalary)
    {
        stationSalary.Comp.PayDayTime = _gameTiming.CurTime + stationSalary.Comp.PayDayTimeOffset;
    }

    public CrewSalaryEntry? GetCrewMemberSalary(StationRecordKey key, string jobId)
    {
        if (!TryComp<CrewMemberStationSalaryComponent>(key.OriginStation, out var salaryComponent))
            return null;

        return !salaryComponent.CachedSalaries.TryGetValue(jobId, out var salary) ? null : new CrewSalaryEntry(salary);
    }

    public CrewSalaryEntry? GetCrewMemberSalary(EntityUid station, string jobId)
    {
        if (!TryComp<CrewMemberStationSalaryComponent>(station, out var salaryComponent))
            return null;

        return !salaryComponent.CachedSalaries.TryGetValue(jobId, out var salary) ? null : new CrewSalaryEntry(salary);
    }
}
