using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Roles.Salary;

[Serializable]
[Prototype("salaries")]
public sealed class SalariesPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public Dictionary<ProtoId<JobPrototype>, int> Salaries = new();
}
