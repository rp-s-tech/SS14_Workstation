using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.RPSX.Administration.Commands.ERT;

[Prototype("ERTGroup")]
public sealed class ERTGroupPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public ResPath Path;

    [DataField]
    public string MessageOnSpawn = default!;

    [DataField]
    public SoundSpecifier? SoundOnSpawn;
}
