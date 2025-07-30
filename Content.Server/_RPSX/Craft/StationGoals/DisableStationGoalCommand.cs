using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.RPSX.Craft.StationGoals;

[AdminCommand(AdminFlags.Adminhelp)]
public sealed class DisableStationGoalCommand : IConsoleCommand
{
    public string Command => "disablestationgoal";
    public string Description => "Отключает цель станции. (вводится до начала раунда)";
    public string Help => "Просто введи до начала раунда";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        IoCManager.Resolve<IEntityManager>().System<StationGoalPaperSystem>().DisableStationGoal();
    }

}
