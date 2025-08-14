using Content.Server.Power.Components;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Objectives.Components;
using Content.Shared.Radio.Components;
using Robust.Shared.Map;

namespace Content.Server.RPSX.GameRules.Pirates.Objectives.ConnectionDisruption;

public sealed partial class ConnectionDisruptionConditionSystem : BasePirateObjective<ConnectionDisruptionConditionComponent>
{
    [Dependency] private readonly RadioSystem _radio = default!;

    protected override void CheckObjectiveCompleted(Entity<ConnectionDisruptionConditionComponent> entity, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;
        if (GetProgress(entity) is not { Owner.Valid: true } piratesProgress)
            return;

        if (piratesProgress.Comp.TargetStation is not { Valid: true } targetStation)
            return;

        var mapId = Transform(targetStation).MapID;
        var inactive = 0;
        var all = 0;
        foreach (var channel in entity.Comp.Channels)
        {
            all += 1;
            if (HasActiveServer(mapId, channel))
                continue;
            inactive += 1;
        }
        args.Progress = inactive / all;
    }

    private bool HasActiveServer(MapId mapId, string channelId)
    {
        var servers = EntityQuery<TelecomServerComponent, EncryptionKeyHolderComponent, ApcPowerReceiverComponent, TransformComponent>();
        foreach (var (_, keys, power, transform) in servers)
        {
            if (transform.MapID == mapId &&
                power.Powered &&
                keys.Channels.Contains(channelId))
            {
                return true;
            }
        }
        return false;
    }
}