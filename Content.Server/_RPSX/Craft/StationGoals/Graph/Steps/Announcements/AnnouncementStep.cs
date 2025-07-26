using Content.Server.Chat.Systems;
using Content.Server.RPSX.Utils;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Announcements;

public sealed partial class AnnouncementStep : Step
{
    [DataField("sender", serverOnly: true, required: true)]
    private string _sender = default!;

    [DataField("messageLoc", serverOnly: true, required: true)]
    private string _messageLoc = default!;

    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var chatSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>();

        var message = Loc.GetString(_messageLoc);

        ChatUtils.SendLocMessageFromCustom(
            chatSystem: chatSystem,
            locCode: message,
            sender: Loc.GetString(_sender),
            stationId: null
        );

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }
}
