using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.RPSX.Administration.Commands.ERT;

[AdminCommand(AdminFlags.Adminhelp)]
public sealed class ERTSpawnCommand : IConsoleCommand
{
    public string Command => "ertspawn";
    public string Description => "Spawn's ERT Team to safe station";
    public string Help => "Usage: ertspawn";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var ertSystem = entityManager.System<ERTSystem>();

        if (shell.Player is not { } session)
            return;

        ertSystem.CallERT(session);
    }
}

