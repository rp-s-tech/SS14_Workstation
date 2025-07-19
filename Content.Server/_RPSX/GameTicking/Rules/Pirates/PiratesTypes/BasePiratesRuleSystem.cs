using Content.Server.RPSX.GameRules.Pirates;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Server.Antag;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Server.RPSX.GameTicking.Rules.Pirates.Starting;
using Content.Shared.GameTicking.Components;

namespace Content.Server.RPSX.GameTicking.Rules.Pirates.PiratesTypes
{
    public abstract class BasePiratesRuleSystem<T> : GameRuleSystem<T> where T : BasePiratesRuleComponent
    {
        [Dependency] private readonly PiratesProgressSystem _progressSystem = default!;

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

        private void OnGetBriefing(Entity<PirateRoleComponent> role, ref GetBriefingEvent args)
        {
            // TODO Different character screen briefing for the 3 nukie types
            args.Append(Loc.GetString("pirates-briefing"));
        }
    }
}
