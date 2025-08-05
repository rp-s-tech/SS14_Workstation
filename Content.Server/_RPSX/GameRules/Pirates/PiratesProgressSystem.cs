using System.Linq;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.RPSX.GameRules.Pirates.Economics;
using Content.Server.Radio.EntitySystems;
using Content.Server.RPSX.GameRules.Pirates.Objectives;
using Content.Server.RPSX.GameRules.Pirates.Objectives.BalanceIncreasing;
using Content.Shared.Radio;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.Mobs.Components;
using Robust.Shared.Utility;
using Content.Server.RoundEnd;
using Content.Shared.Mobs;
using Content.Shared.Zombies;

namespace Content.Server.RPSX.GameRules.Pirates;

public sealed partial class PiratesProgressSystem : EntitySystem
{
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly PirateEconomicsSystem _economicsSystem = default!;
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;

    private readonly EntProtoId _piratesProgressHolder = "PiratesProgressHolder";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PiratesProgressComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<PirateComponent, MobStateChangedEvent>(OnPirateMobStateChanged);
        SubscribeLocalEvent<PirateComponent, EntityZombifiedEvent>(OnPirateZombified);
        SubscribeLocalEvent<PirateComponent, ComponentRemove>(OnPirateComponentRemoved);
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

    private void CheckRoundShouldEnd(Entity<PiratesProgressComponent> entity)
    {
        var (freePiratesCount, mainProgress, mainObjCount, addProgress, addObjCount) = CalculateProgress(entity);
        var allObjectivesDone = mainProgress == mainObjCount && addProgress == addObjCount;
        var noProgress = mainProgress == 0f && addProgress == 0f;
        var allPiratesFree = freePiratesCount == entity.Comp.StartedPirates.Count;

        var state = PiratesWinState.None;

        if (freePiratesCount == 0)
        {
            if (noProgress) state = PiratesWinState.CrewMajor;
            else state = PiratesWinState.CrewMinor;
        }
        else if (entity.Comp.RoundCanBeEnded)
        {
            if (allObjectivesDone)
            {
                if (allPiratesFree)
                {
                    state = entity.Comp.Balance - GetBalanceGoal(entity) >= 50
                        ? PiratesWinState.PiratesMajor
                        : PiratesWinState.PiratesMinor;
                }
                else
                {
                    state = PiratesWinState.PiratesMinor;
                }
            }
            else if (mainProgress == mainObjCount && allPiratesFree)
            {
                state = PiratesWinState.PiratesMinor;
            }
            else
            {
                state = PiratesWinState.Neutral;
            }
        }
        if (state != PiratesWinState.None)
            SetWinState(entity, state);
    }

    private int GetBalanceGoal(Entity<PiratesProgressComponent> entity)
    {
        var goal = EntityQuery<BalanceIncreasingObjectiveComponent, PirateObjectiveComponent>()
            .Where(p => p.Item2.PiratesProgress == entity.Owner).FirstOrNull();

        if (goal != null)
            return goal.Value.Item1.Goal;

        return 0;
    }

    private (int, double, double, double, double) CalculateProgress(Entity<PiratesProgressComponent> entity)
    {
        var freePiratesCount = entity.Comp.StartedPirates
            .Count(p => TryComp<MobStateComponent>(p, out var mobState)
                        && mobState.CurrentState == Shared.Mobs.MobState.Alive
                        && Transform(p).GridUid == entity.Comp.PiratesShuttle);

        double mainObjCount = entity.Comp.Objectives.Count(p =>
            TryComp<PirateObjectiveComponent>(p, out var objective)
            && objective.Priority == "main");
        double addObjCount = entity.Comp.Objectives.Count(p =>
            TryComp<PirateObjectiveComponent>(p, out var objective)
            && objective.Priority == "additional");

        var (mainProgress, addProgress) = GetObjectivesProgress(entity);

        return (freePiratesCount, mainProgress, mainObjCount, addProgress, addObjCount);
    }

    private void SetWinState(Entity<PiratesProgressComponent> entity, PiratesWinState state)
    {
        entity.Comp.PiratesWinState = state;

        if (state <= PiratesWinState.Neutral)
            _roundEndSystem.EndRound();
    }

    #endregion

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PiratesProgressComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.PiratesWinState != PiratesWinState.None)
                continue;

            if (component.ObjectivesSpawnTime <= _timing.CurTime && !component.Objectives.Any())
            {
                SpawnObjectives((uid, component));
            }
            else
            {
                CheckObjectives((uid, component));
            }
        }
    }

    private void SendMessageFromHead(EntityUid progress, string message)
    {
        _radio.SendRadioMessage(progress, message, _prototypeManager.Index<RadioChannelPrototype>("Pirates"), progress);
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

    private void OnPirateMobStateChanged(Entity<PirateComponent> entity, ref MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead && TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent))
        {
            CheckRoundShouldEnd((entity.Comp.PiratesProgress, progressComponent));
        }
    }

    private void OnPirateZombified(Entity<PirateComponent> entity, ref EntityZombifiedEvent args)
    {
        RemCompDeferred(entity, entity.Comp);
    }

    private void OnPirateComponentRemoved(Entity<PirateComponent> entity, ref ComponentRemove args)
    {
        if (TryComp<PiratesProgressComponent>(entity.Comp.PiratesProgress, out var progressComponent))
        {
            CheckRoundShouldEnd((entity.Comp.PiratesProgress, progressComponent));
        }
    }
}
