using Content.Server.Fax;
using Content.Shared.Fax.Components;
using Content.Shared.Paper;

namespace Content.Server.RPSX.Craft.StationGoals.Graph.Steps.Announcements;

public sealed partial class PrintGoalToFaxStep : Step
{
    [DataField("messageLoc", serverOnly: true, required: true)]
    private string _messageLoc = default!;
    internal override ExecuteState ExecuteStep(Dictionary<StepDataKey, object> results, StationGoalPaperSystem system)
    {
        var goal = system.CurrentGoal;
        if (goal == null)
        {
            system.Logger.RootSawmill.Debug($"Step: {Name} interruped goal is null");
            return ExecuteState.Interrupted;
        }

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
        var faxSystem = entitySystemManager.GetEntitySystem<FaxSystem>();
        var faxes = entityManager.EntityQueryEnumerator<FaxMachineComponent>();

        while (faxes.MoveNext(out var uid, out var fax))
        {
            if (!fax.ReceiveStationGoal)
                continue;

            var printout = new FaxPrintout(
                Loc.GetString(_messageLoc),
                Loc.GetString("station-goal-fax-paper-name"),
                null,
                null,
                "paper_stamp-centcom",
                new List<StampDisplayInfo> { GetStampDisplayInfo() }
            );
            faxSystem.Receive(uid, printout, null, fax);
        }

        system.Logger.RootSawmill.Debug($"Step: {Name} finished success");
        return ExecuteState.Finished;
    }

    private StampDisplayInfo GetStampDisplayInfo()
    {
        var stampedName = Loc.GetString("stamp-component-stamped-name-centcom");
        var stampedColor = Color.TryFromHex("#006600");

        return new StampDisplayInfo
        {
            StampedName = stampedName,
            StampedColor = stampedColor ?? Color.DarkGreen
        };
    }
}
