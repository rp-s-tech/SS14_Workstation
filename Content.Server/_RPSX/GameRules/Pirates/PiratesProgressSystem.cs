using System.Linq;
using Content.RPSX.Shared.GameRules.Pirates;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.RPSX.Server.GameRules.Pirates;

public sealed partial class PiratesProgressSystem : SharedPiratesProgressSystem
{
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PiratesProgressComponent, MapInitEvent>(OnMapInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PiratesProgressComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            var progress = (uid, component);
            CheckObjectives(progress);
            if (progress.component.Objectives.Any()) return;
            if (progress.component.ObjectivesSpawnTime > _timing.CurTime) return;
            SpawnObjectives(progress);
        }
    }

    private void OnMapInit(Entity<PiratesProgressComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.ObjectivesSpawnTime = _timing.CurTime + TimeSpan.FromMinutes(5);
        var locMessage = entity.Comp.GamePlay switch
        {
            PiratesGamePlay.Loud => "",
            PiratesGamePlay.Silent => "",
            _ => ""
        };
        var message = Loc.GetString(locMessage);
        SendMessageFromHead(entity, message);
    }

    private void SendMessageFromHead(Entity<PiratesProgressComponent> progress, string message)
    {
        _radio.SendRadioMessage(progress, message, _prototypeManager.Index<RadioChannelPrototype>("Pirates"), progress);
    }
}
