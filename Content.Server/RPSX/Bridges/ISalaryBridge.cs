using Content.Shared.RPSX.Roles.Salary;
using Content.Shared.StationRecords;

namespace Content.Server.RPSX.Bridges;

public interface ISalaryBridge
{
    CrewSalaryEntry? GetCrewMemberSalary(StationRecordKey key, string jobId);

    CrewSalaryEntry? GetCrewMemberSalary(EntityUid station, string jobId);
}

public sealed class StubSalaryBridge : ISalaryBridge
{
    public CrewSalaryEntry? GetCrewMemberSalary(StationRecordKey key, string jobId) => null;
    public CrewSalaryEntry? GetCrewMemberSalary(EntityUid station, string jobId) => null;
}
