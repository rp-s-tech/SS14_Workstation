using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Content.Server.RPSX.RandomTeleport;
using Content.Shared.Maps;
using Robust.Shared.Audio;


namespace Content.Server.RPSX.EntityEffects.Effects;

public sealed partial class RandomTeleportEffect : EntityEffect
{
    [DataField]
    public float Radius = 5f;

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-random-teleport", ("chance", Probability));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var transformSys = args.EntityManager.System<SharedTransformSystem>();
        var audioSys = args.EntityManager.System<SharedAudioSystem>();
        var randomTeleportSys = args.EntityManager.System<RandomTeleportSystem>();
        var mapSys = args.EntityManager.System<SharedMapSystem>();
        var mapManager = IoCManager.Resolve<IMapManager>();

        EntityCoordinates? targetCoords = null;

        for (int i = 0; i < 5; i++)
        {
            var potentialCoords  = randomTeleportSys.GetRandomCoordinates(args.TargetEntity, Radius);
            if (!potentialCoords.HasValue)
                continue;

            var potentialMapCoords  = transformSys.ToMapCoordinates(potentialCoords.Value);

            if (!mapManager.TryFindGridAt(potentialMapCoords, out var gridUid, out var grid) ||
                !mapSys.TryGetTileRef(gridUid, grid, potentialCoords.Value, out var tileRef))
                continue;

            if (!tileRef.Tile.IsSpace())
            {
                targetCoords = potentialCoords;
                break;
            }
        }

        if (targetCoords.HasValue)
        {
            transformSys.SetCoordinates(args.TargetEntity, targetCoords.Value);
            audioSys.PlayPvs(Sound, args.TargetEntity);

        }
    }
}
