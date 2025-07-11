using Robust.Shared.Serialization;

namespace Content.Shared.Body.Organ;

[Serializable, NetSerializable]
[DataRecord]
public sealed record OrganSlot(string Id, NetEntity Parent, OrganType? Type)
{
    public NetEntity? Child { get; set; }

    /// <summary>
    /// an attached surgical tool on the body part slot (such as a Torniquet)
    /// </summary>
    public NetEntity? Attachment { get; set; }

    public bool Cauterised = false;
}
