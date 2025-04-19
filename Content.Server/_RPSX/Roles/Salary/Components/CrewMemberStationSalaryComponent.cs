using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RPSX.Roles.Salary.Components;

[RegisterComponent]
public sealed partial class CrewMemberStationSalaryComponent : Component
{
    [DataField(required: true)]
    public ProtoId<SalariesPrototype> Salaries;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan PayDayTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan PayDayTimeOffset = TimeSpan.FromMinutes(30);

    [DataField]
    public Dictionary<ProtoId<JobPrototype>, int> CachedSalaries = new();
}
