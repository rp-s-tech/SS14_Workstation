using Content.Shared.Objectives;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.StealSpecials;

[RegisterComponent]
public sealed partial class StealSpecialsConditionComponent : Component
{
    [DataField]
    public EntityUid? ObjectiveItem;

    [DataField]
    public HashSet<ProtoId<StealTargetGroupPrototype>> StealTargetGroups = new HashSet<ProtoId<StealTargetGroupPrototype>>
    {
        "Hypospray",
        "HandheldCrewMonitor",
        "ClothingOuterHardsuitRd",
        "HandTeleporter",
        "ClothingShoesBootsMagAdv",
        "BoxFolderQmClipboard",
        "ClothingHandsKnuckleDustersQM",
        "Teg",
        "ToiletGoldenDirtyWater",
    };

}
