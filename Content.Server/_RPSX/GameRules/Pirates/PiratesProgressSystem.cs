using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.RPSX.GameRules.Pirates.Economics;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem : EntitySystem
{
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly PirateEconomicsSystem _economicsSystem = default!;

    private readonly EntProtoId _piratesProgressHolder = "PiratesProgressHolder";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PiratesProgressComponent, MapInitEvent>(OnMapInit);
    }

    #region Rule
    public Entity<PiratesProgressComponent> CreateProgress()
    {
        var progress = Spawn(_piratesProgressHolder);
        var progressComp = EnsureComp<PiratesProgressComponent>(progress);
        return (progress, progressComp);
    }

    public void SetGamePlay(Entity<PiratesProgressComponent?> progress)
    {
        if (!Resolve(progress.Owner, ref progress.Comp)) return;

        if (_playerManager.PlayerCount > 40)
            progress.Comp.GamePlay = GetRandomEnumValue<PiratesGamePlay>();

        Dirty(progress);
    }

    public void DistributeExtraDublons(Entity<PiratesProgressComponent?> progress)
    {
        if (!Resolve(progress.Owner, ref progress.Comp)) return;

        if (progress.Comp.GamePlay == PiratesGamePlay.Silent) return;
        _economicsSystem.ChangePiratesBalance((progress.Owner, progress.Comp), 100);
    }

    public string GetGameModeResultLine(Entity<PiratesProgressComponent?> progress)
    {
        if (!Resolve(progress.Owner, ref progress.Comp)) return "";

        var message = progress.Comp.PiratesWinState switch
        {
            PiratesWinState.PiratesMajor => "",
            PiratesWinState.PiratesMinor => "",
            PiratesWinState.Neutral => "",
            PiratesWinState.CrewMinor => "",
            PiratesWinState.CrewMajor => "",
            _ => ""
        };
        return Loc.GetString(message);
    }

    private static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Shared.Next(values.Length))!;
    }
    #endregion

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PiratesProgressComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            var progress = (uid, component);
            if (progress.component.Objectives.Any())
            {
                CheckObjectives(progress);
                continue;
            }

            if (progress.component.ObjectivesSpawnTime > _timing.CurTime)
                continue;

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
