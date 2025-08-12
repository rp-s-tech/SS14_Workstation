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

    public static readonly CVarDef<bool> IsHeartAttackEnabled =
        CVarDef.Create("surgery.heart_attack_enabled", false);

    public static readonly CVarDef<bool> IsHeartStrainEnabled =
        CVarDef.Create("surgery.heart_strain_enabled", false);

    public static readonly CVarDef<bool> IsSeveredLimbEnabled =
        CVarDef.Create("surgery.severed_limb_enabled", true);

    public static readonly CVarDef<float> SurgeryAngleFactor =
        CVarDef.Create("surgery.angle_factor", 0.1f, CVar.SERVER);

    public static readonly CVarDef<float> SurgeryDistanceFactor =
        CVarDef.Create("surgery.distance_factor", 0.15f, CVar.SERVER);

    public static readonly CVarDef<float> SurgeryPartRemoveProbabilty =
        CVarDef.Create("surgery.part_remove_probability", 0.2f, CVar.SERVER);
}
