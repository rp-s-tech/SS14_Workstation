using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class RPSXCCVars : CVars
{
    public static readonly CVarDef<string> SponsorsApiUrl =
        CVarDef.Create("sponsor.api_url", "" , CVar.SERVERONLY); //"127.0.0.1:5000"
}
