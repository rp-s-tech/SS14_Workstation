using Robust.Shared.Configuration;

namespace Content.Shared.RPSX.CCVars;

public sealed partial class RPSXCCVars
{
    public static readonly CVarDef<bool> SurgeryEnabled =
        CVarDef.Create("surgery.enabled", false, CVar.REPLICATED | CVar.SERVER);
}