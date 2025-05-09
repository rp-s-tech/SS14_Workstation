using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Server.RPSX.CCvars;

[CVarDefs]
public sealed class RPSXCCVars : CVars
{
    public static readonly CVarDef<bool> IsFakeNumbersEnabled =
        CVarDef.Create("config.is_fake_numbers_enabled", true, CVar.SERVERONLY);

    public static readonly CVarDef<int> NarsiMinPlayers =
        CVarDef.Create("narsi.min_players", 30, CVar.SERVERONLY);

    public static readonly CVarDef<int> VampireMinPlayers =
        CVarDef.Create("vampire.min_players", 30, CVar.SERVERONLY);

    public static readonly CVarDef<int> RatvarMaxRighteousCount =
        CVarDef.Create("ratvar.max_righteous_count", 6);

}
