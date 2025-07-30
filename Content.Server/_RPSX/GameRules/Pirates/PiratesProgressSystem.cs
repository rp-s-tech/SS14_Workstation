using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.RPSX.GameRules.Pirates.Economics;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.Mobs.Components;

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
        SubscribeLocalEvent<BasePirateComponent, MapInitEvent>(OnBaseMapInit);
    }

    #region Rule
    public Entity<PiratesProgressComponent> CreateProgress()
    {
        var progress = Spawn(_piratesProgressHolder);
        var progressComp = EnsureComp<PiratesProgressComponent>(progress);
        progressComp.CompOwner = progress;
        Dirty(progress, progressComp);
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
            if (component.Objectives.Any() || component.PiratesWinState != PiratesWinState.None)
                continue;

            if (component.ObjectivesSpawnTime > _timing.CurTime)
                continue;

            SpawnObjectives((uid, component));
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

    private void OnBaseMapInit(Entity<BasePirateComponent> entity, ref MapInitEvent args)
    {
        var progress = EntityQuery<PiratesProgressComponent>().Where(p => p.PiratesOutpostMap == Transform(entity).MapUid).FirstOrDefault();
        if (progress == null)
        {
            QueueDel(entity);
            return;
        }
        entity.Comp.PiratesProgress = progress.CompOwner;
        entity.Comp.CompOwner = entity.Owner;

        if (HasComp<PirateComponent>(entity.Owner))
        {
            progress.StartedPirates.Add(entity.Owner);
        }
        DirtyField(progress.CompOwner, progress, nameof(PiratesProgressComponent.StartedPirates));
    }

    private void SendMessageFromHead(Entity<PiratesProgressComponent> progress, string message)
    {
        _radio.SendRadioMessage(progress, message, _prototypeManager.Index<RadioChannelPrototype>("Pirates"), progress);
    }

    private void CalculateWinState(Entity<PiratesProgressComponent> entity)
    {
        var alivePiratesCount = EntityQuery<PirateComponent>()
            .Where(p => TryComp<MobStateComponent>(p.CompOwner, out var mobState)
                && mobState.CurrentState == Shared.Mobs.MobState.Alive
                && p.PiratesProgress == entity.Owner)
            .Count();

        if (alivePiratesCount == 0)
        {
            entity.Comp.PiratesWinState = PiratesWinState.CrewMajor;
            return;
        }
    }
}
