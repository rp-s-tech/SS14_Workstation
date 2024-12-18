using Content.Shared.EntityEffects;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.RPSX.EntityEffects.Effects;

public sealed partial class ToxinTeleport : EntityEffect
{
    [DataField]
    public float TeleportRadius;

    [DataField]
    public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    [DataField]
    public int MaxEffectSkip;

    private int _curEffectSkip;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => null;

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs)
            return;

        if (_curEffectSkip != 0)
        {
            _curEffectSkip--;
            return;
        }

        var uid = reagentArgs.TargetEntity;
        TeleportEntity(uid, args.EntityManager);

        _curEffectSkip = MaxEffectSkip;
    }

    private void TeleportEntity(EntityUid target, IEntityManager entityManager)
    {
        if (!entityManager.TryGetComponent<TransformComponent>(target, out var transform))
            return;

        var random = IoCManager.Resolve<IRobustRandom>();
        var mapManager = IoCManager.Resolve<IMapManager>();
        var audio = entityManager.System<AudioSystem>();
        var transformSystem = entityManager.System<SharedTransformSystem>();
        var entityCoords = transformSystem.ToMapCoordinates(transform.Coordinates);

        for (var i = 0; i < 20; i++)
        {
            var distance = TeleportRadius * MathF.Sqrt(random.NextFloat());
            var targetCoords = entityCoords.Offset(random.NextAngle().ToVec() * distance);
            if (!mapManager.TryFindGridAt(targetCoords, out _, out _))
                continue;

            transformSystem.SetWorldPosition(target, targetCoords.Position);
            audio.PlayPvs(TeleportSound, target);
            break;
        }
    }
}
