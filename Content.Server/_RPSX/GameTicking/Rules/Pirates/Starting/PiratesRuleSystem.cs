using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server.Administration.Logs;
using Content.Shared.GameTicking.Components;
using Content.Shared.Random;
using Content.Shared.Database;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking;

namespace Content.Server.RPSX.GameTicking.Rules.Pirates.Starting;

public sealed class PiratesRuleSystem : GameRuleSystem<PiratesRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;

    private string _ruleCompName = default!;
    private ProtoId<WeightedRandomPrototype> _randomProtos = "Pirates";

    public override void Initialize()
    {
        base.Initialize();
        _ruleCompName = Factory.GetComponentName<GameRuleComponent>();
    }

    protected override void Added(EntityUid uid, PiratesRuleComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);

        if (!TryPickPreset(_randomProtos, out var rule))
        {
            Log.Error($"{ToPrettyString(uid)} failed to pick any preset. Removing rule.");
            Del(uid);
            return;
        }

        Log.Info($"Selected {rule} as the secret preset.");
        _adminLogger.Add(LogType.EventStarted, $"Selected {rule} as the secret preset.");

        EntityUid ruleEnt;

        if (GameTicker.RunLevel <= GameRunLevel.InRound)
            ruleEnt = GameTicker.AddGameRule(rule);
        else
            GameTicker.StartGameRule(rule, out ruleEnt);

        component.AdditionalGameRules.Add(ruleEnt);
    }

    protected override void Ended(EntityUid uid, PiratesRuleComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        foreach (var rule in component.AdditionalGameRules)
        {
            GameTicker.EndGameRule(rule);
        }
    }

    private bool TryPickPreset(ProtoId<WeightedRandomPrototype> weights, [NotNullWhen(true)] out EntProtoId? rule)
    {
        var options = _prototypeManager.Index(weights).Weights.ShallowClone();
        var players = GameTicker.ReadyPlayerCount();

        EntProtoId? selectedRule = null;
        var sum = options.Values.Sum();
        while (options.Count > 0)
        {
            var accumulated = 0f;
            var rand = _random.NextFloat(sum);
            foreach (var (key, weight) in options)
            {
                accumulated += weight;
                if (accumulated < rand)
                    continue;

                selectedRule = key;

                options.Remove(key);
                sum -= weight;
                break;
            }

            if (CanPick(selectedRule, players))
            {
                rule = selectedRule;
                return true;
            }

            if (selectedRule != null)
                Log.Info($"Excluding {selectedRule} from secret preset selection.");
        }

        rule = null;
        return false;
    }

    /// <summary>
    /// Can the given preset be picked, taking into account the currently available player count?
    /// </summary>
    private bool CanPick([NotNullWhen(true)] EntProtoId? selected, int players)
    {
        if (selected == null)
            return false;

        if (!_prototypeManager.TryIndex(selected, out var rule)
                || !rule.TryGetComponent(_ruleCompName, out GameRuleComponent? ruleComp))
        {
            Log.Error($"Encountered invalid rule {selected}");
            return false;
        }

        if (ruleComp.MinPlayers > players && ruleComp.CancelPresetOnTooFewPlayers)
            return false;

        return true;
    }
}

