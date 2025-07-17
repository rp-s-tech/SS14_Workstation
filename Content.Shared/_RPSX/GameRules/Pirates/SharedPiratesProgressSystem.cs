using Content.RPSX.Shared.GameRules.Pirates.Economics;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.RPSX.Shared.GameRules.Pirates;

public abstract class SharedPiratesProgressSystem : EntitySystem
{
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    [Dependency] private readonly PirateEconomicsSystem _economicsSystem = default!;

    private readonly EntProtoId _piratesProgressHolder = "PiratesProgressHolder";
    #region Rule
    public Entity<PiratesProgressComponent> CreateProgress()
    {
        var progress = Spawn(_piratesProgressHolder);
        var progressComp = EnsureComp<PiratesProgressComponent>(progress);
        return (progress, progressComp);
    }

    public void SetGamePlay(Entity<PiratesProgressComponent> progress)
    {
        if (_playerManager.PlayerCount > 40)
            progress.Comp.GamePlay = GetRandomEnumValue<PiratesGamePlay>();
    }

    public void DistributeExtraDublons(Entity<PiratesProgressComponent> progress)
    {
        if (progress.Comp.GamePlay == PiratesGamePlay.Silent) return;
        _economicsSystem.ChangePiratesBalance(progress, 100);
    }

    public string GetGameModeResultLine(Entity<PiratesProgressComponent> progress)
    {
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
    #endregion

    private static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Shared.Next(values.Length))!;
    }

}
