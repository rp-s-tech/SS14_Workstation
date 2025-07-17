using System.Linq;
using Content.RPSX.Server.GameRules.Pirates;
using Content.RPSX.Shared.GameRules.Pirates;
using Content.Server.Antag;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Server.Station.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.RPSX.Server.GameTicking.Rules.Pirates;

public sealed class PiratesRuleSystem : GameRuleSystem<PiratesRuleComponent>
{
    [Dependency] private readonly SharedPiratesProgressSystem _progressSystem = default!;
    private Entity<PiratesProgressComponent> _progress;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PiratesRuleComponent, AntagSelectionEnd>(OnAntagSelectionEnd);
        SubscribeLocalEvent<PirateRoleComponent, GetBriefingEvent>(OnGetBriefing);
    }

    protected override void Added(EntityUid uid, PiratesRuleComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);
        if (MetaData(uid).EntityPrototype is not EntityPrototype prototype) return;
        _progress = _progressSystem.CreateProgress();
        _progress.Comp.PiratesGamerule = prototype.ID;
    }

    protected override void Started(EntityUid uid, PiratesRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        GetOutpost(uid);
        if (!TryGetRandomStation(out var station)) return;
        _progress.Comp.TargetStation = station.Value;
    }

    private void OnAntagSelectionEnd(EntityUid uid, PiratesRuleComponent component, ref AntagSelectionEnd args)
    {
        _progressSystem.SetGamePlay(_progress);
        _progressSystem.DistributeExtraDublons(_progress);
    }

    protected override void ActiveTick(EntityUid uid, PiratesRuleComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        // if (component.WinStateStatus == WinState.CultistWon && _gameTiming.CurTime > component.RoundEndAt)
        // {
        //     _roundEndSystem.EndRound();
        //     component.WinStateStatus = WinState.Idle;
        // }

        // if (component.WinStateStatus != WinState.NarsiSummoning || _gameTiming.CurTime <= component.NarsiRepeatSoundAt)
        //     return;

        // component.NarsiRepeatSoundAt = _gameTiming.CurTime + TimeSpan.FromSeconds(35f);

        // _soundSystem.StopStationEventMusic(component.RuneSource, StationEventMusicType.Narsi);
        // _soundSystem.DispatchStationEventMusic(component.RuneSource, component.NarsiSummonSound,
        //     StationEventMusicType.Narsi);
    }

    protected override void AppendRoundEndText(EntityUid uid,
        PiratesRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);

        var result = _progressSystem.GetGameModeResultLine(_progress);
        args.AddLine(result);
    }

    private void OnGetBriefing(Entity<PirateRoleComponent> role, ref GetBriefingEvent args)
    {
        // TODO Different character screen briefing for the 3 nukie types
        args.Append(Loc.GetString("pirates-briefing"));
    }

    private void GetOutpost(Entity<RuleGridsComponent?> ent)
    {
        if(!Resolve(ent, ref ent.Comp, false))
            return;

        var outpost = ent.Comp.MapGrids.Where(e => !HasComp<PiratesShuttleComponent>(e)).FirstOrNull();
        _progress.Comp.PiratesOutpost = outpost;
    }
}
