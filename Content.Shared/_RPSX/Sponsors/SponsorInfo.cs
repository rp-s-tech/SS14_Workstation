using System.IO;
using System.Text.Json.Serialization;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.RPSX.Sponsors;

[Serializable, NetSerializable]
public sealed class SponsorInfo
{
    [JsonPropertyName("tier")]
    public string? TierId { get; set; }
}

[Serializable, NetSerializable]
public sealed class AllSponsorInfo
{
    public int AvailableItems;
    public bool RoleTimeByPass;
    public List<string> AllowedMarkings = [];
    public List<string> AllowedLoadouts = [];
    public List<string> AllowedSpecies = [];
    public bool HavePriorityJoin;
    public List<string> PetCategories = [];
    public List<string> Ghosts = [];
}
/// <summary>
/// Server sends sponsoring info to client on connect only if user is sponsor
/// </summary>
public sealed class MsgSponsorInfo : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public AllSponsorInfo? TierId;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var isSponsor = buffer.ReadBoolean();
        buffer.ReadPadBits();
        if (!isSponsor)
            return;
        var length = buffer.ReadVariableInt32();
        using var stream = new MemoryStream(length);
        buffer.ReadAlignedMemory(stream, length);
        serializer.DeserializeDirect(stream, out TierId);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(TierId != null);
        buffer.WritePadBits();
        if (TierId == null)
            return;
        var stream = new MemoryStream();
        serializer.SerializeDirect(stream, TierId);
        buffer.WriteVariableInt32((int)stream.Length);
        buffer.Write(stream.AsSpan());
    }
}
