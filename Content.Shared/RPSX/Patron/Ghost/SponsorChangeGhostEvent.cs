using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Patron.Ghost;

[Serializable, NetSerializable]
public sealed class SponsorChangeGhostEvent(string id) : BoundUserInterfaceMessage
{
    public string Id = id;
}

[Serializable, NetSerializable]
public enum SponsorGhostInterfaceKey
{
    Key,
}
