using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class RPSXCCVars : CVars
{

    /// <summary>
    ///     Enable discord verify.
    /// </summary>
    public static readonly CVarDef<bool> DiscordAuthEnabled =
        CVarDef.Create("discord.auth_enabled", false, CVar.NOTIFY | CVar.REPLICATED);

    /// <summary>
    ///     Discord verify server.
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthServer =
        CVarDef.Create("discord.api_url", "" , CVar.SERVERONLY); //"127.0.0.1:5000"

    /// <summary>
    ///     Enable discord verify.
    /// </summary>
    public static readonly CVarDef<string> DiscordAHelpIcon =
        CVarDef.Create("discord.ahelp_webhook_apiicon", ":desktop:" , CVar.SERVERONLY);


    /// <summary>
    ///     Enable discord verify.
    /// </summary>
    public static readonly CVarDef<string> DiscordAHelpColor =
        CVarDef.Create("discord.ahelp_webhook_color", "pink" , CVar.SERVERONLY);
}
