using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.ConnectionDisruption
{
    public sealed partial class ConnectionDisruptionConditionComponent : BasePirateObjectiveComponent
    {
        [DataField]
        public HashSet<ProtoId<RadioChannelPrototype>> Channels = new()
        {
            "Common",
            "Command",
            "Engineering",
            "Medical",
            "Science",
            "Security",
            "Service",
            "Supply"
        };
    }
}