using Content.Server.Chat.Systems;

namespace Content.Server.RPSX.Utils;

public static class ChatUtils
{
    public static void SendMessageFromCentcom(ChatSystem chatSystem, string message, string sender, EntityUid? stationId)
    {
        SendMessage(
            chatSystem: chatSystem,
            message: message,
            sender: sender,
            stationId: stationId
        );
    }

    public static void SendLocMessageFromCentcom(ChatSystem chatSystem, string locCode, EntityUid? stationId)
    {
        var message = Loc.GetString(locCode);
        if (message == null)
        {
            return;
        }

        SendMessageFromCentcom(chatSystem, (string) message, "Центральное командование", stationId);
    }

    public static void SendLocMessageFromCustom(ChatSystem chatSystem, string locCode, string sender, EntityUid? stationId)
    {
        var message = Loc.GetString(locCode);
        SendMessage(
            chatSystem: chatSystem,
            message: message,
            sender: sender,
            stationId: stationId
        );
    }

    private static void SendMessage(ChatSystem chatSystem, string message, string sender, EntityUid? stationId)
    {
        if (stationId == null)
        {
            chatSystem.DispatchGlobalAnnouncement(
                message: message,
                sender: sender,
                playSound: true,
                colorOverride: Color.Yellow
            );

            return;
        }

        chatSystem.DispatchStationAnnouncement(
            source: (EntityUid) stationId,
            message: message,
            sender: sender,
            playDefaultSound: true,
            colorOverride: Color.Yellow
        );
    }
}
