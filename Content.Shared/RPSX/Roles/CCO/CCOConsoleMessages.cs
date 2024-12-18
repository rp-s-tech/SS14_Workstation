using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Roles.CCO;

[Serializable, NetSerializable]
public sealed class CcoConsoleSendAnnouncementMessage(string message) : BoundUserInterfaceMessage
{
    public readonly string Message = message;
}

[Serializable, NetSerializable]
public sealed class CcoConsoleOpenCrewManifestMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class CcoConsoleSendSpecialSquadMessage(string squadId) : BoundUserInterfaceMessage
{
    public readonly string SquadId = squadId;
}

[Serializable, NetSerializable]
public sealed class CcoConsoleSendEmergencyShuttleMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class CcoConsoleCancelEmergencyShuttleMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class CcoConsoleStationSelected(NetEntity station) : BoundUserInterfaceMessage
{
    public readonly NetEntity Station = station;
}
