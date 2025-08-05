using Content.Server.RPSX.GameRules.Pirates;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Antag;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Utility;

namespace Content.Server.RPSX.GameTicking.Rules.Pirates.PiratesTypes
{
    public abstract class BasePiratesRuleSystem<T> : GameRuleSystem<T> where T : BasePiratesRuleComponent
    {
        [Dependency] private readonly PiratesProgressSystem _progressSystem = default!;
        [Dependency] private readonly MapSystem _mapSystem = default!;
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<T, AntagSelectionEnd>(OnAntagSelectionEnd);
            SubscribeLocalEvent<PirateRoleComponent, GetBriefingEvent>(OnGetBriefing);
        }

        protected override void Started(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleStartedEvent args)
        {
            base.Started(uid, component, gameRule, args);

            var progress = _progressSystem.CreateProgress();
            _progressSystem.SetGamePlay(progress.Owner);

            progress.Comp.ObjectivesToSpawn = progress.Comp.GamePlay switch
            {
                PiratesGamePlay.Silent => component.ObjectivesSilent,
                PiratesGamePlay.Loud => component.ObjectivesLoud,
                _ => component.ObjectivesSilent
            };
            progress.Comp.PiratesShuttle = GetShuttle(uid);

            if (!TryGetRandomStation(out var station)) return;
            progress.Comp.TargetStation = station.Value;

            Dirty(progress);
        }

        private void OnAntagSelectionEnd(EntityUid uid, T component, ref AntagSelectionEnd args)
        {
            _progressSystem.DistributeExtraDublons(component.Progress);
        }

        protected override void AppendRoundEndText(EntityUid uid, T component, GameRuleComponent gameRule, ref RoundEndTextAppendEvent args)
        {
            base.AppendRoundEndText(uid, component, gameRule, ref args);

            var result = _progressSystem.GetGameModeResultLine(component.Progress);
            args.AddLine(result);
        }

        //     protected override void AppendRoundEndText(EntityUid uid,
        //     NukeopsRuleComponent component,
        //     GameRuleComponent gameRule,
        //     ref RoundEndTextAppendEvent args)
        // {
        //     var winText = Loc.GetString($"nukeops-{component.WinType.ToString().ToLower()}");
        //     args.AddLine(winText);

        //     foreach (var cond in component.WinConditions)
        //     {
        //         var text = Loc.GetString($"nukeops-cond-{cond.ToString().ToLower()}");
        //         args.AddLine(text);
        //     }

        //     args.AddLine(Loc.GetString("nukeops-list-start"));

        //     var antags = _antag.GetAntagIdentifiers(uid);

        //     foreach (var (_, sessionData, name) in antags)
        //     {
        //         args.AddLine(Loc.GetString("nukeops-list-name-user", ("name", name), ("user", sessionData.UserName)));
        //     }
        // }

        private void OnGetBriefing(Entity<PirateRoleComponent> role, ref GetBriefingEvent args)
        {
            // TODO Different character screen briefing for the 3 nukie types
            args.Append(Loc.GetString("pirates-briefing"));
        }

        private EntityUid? GetShuttle(Entity<RuleGridsComponent?> rule)
        {
            if (!Resolve(rule.Owner, ref rule.Comp)) return null;

            var gridUid = rule.Comp.MapGrids.FirstOrNull();
            if (!gridUid.HasValue) return null;

            return gridUid;
        }
    }
}
