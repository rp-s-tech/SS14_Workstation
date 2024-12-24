using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Content.Shared.RPSX.CCVars;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server.RPSX.Commands;

[AnyCommand]
public sealed class DropPlayTime : IConsoleCommand
{
    [Dependency] private readonly PlayTimeTrackingManager _playtime = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    public string Command => "dropplaytime";
    public string Description => "Сбрасывает все наигранное игровое время";
    public string Help => "Введите команду dropplaytime true чтобы сбросить все наигранное время";
    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
            return;

        if (!_cfg.GetCVar(RPSXCCVars.IsDropTimeEnabled))
        {
            shell.WriteLine("Функционал отключен");
            return;
        }

        if (args.Length < 1)
        {
            shell.WriteLine("Для сброса времени напишите команду следующим образом: dropplaytime true");
            return;
        }

        if (!args[0].Equals("true"))
            return;

        _playtime.ClearAllTime(player);
        shell.WriteLine("Время сброшено, может потребоваться перезаход, для обновления");
    }
}
