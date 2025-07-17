using Content.RPSX.Shared.GameRules.Pirates;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Random;

namespace Content.RPSX.Server.GameTicking.Rules.Pirates;

public sealed class BasePiratesRuleSystem : GameRuleSystem<BasePiratesRuleComponent>
{
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private readonly HashSet<string> _piratesGamerules = ["BasicPirates", "CyberCorpPirates", "IndustrialPirates", "DSDPirates",
        "ClassicPirates"];

    protected override void Started(EntityUid uid, BasePiratesRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        var piratesGamerule = _random.Pick(_piratesGamerules);
        Log.Info($"Starting gamerule {piratesGamerule} as a subgamemode of {ToPrettyString(uid):rule}");
        _gameTicker.AddGameRule(piratesGamerule);
    }
}
