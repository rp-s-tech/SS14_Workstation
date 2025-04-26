using Content.Server.RPSX.GameTicking.Rules.Narsi;
using Content.Server.RPSX.GameTicking.Rules.Ratvar;
using Content.Server.RPSX.GameTicking.Rules.Vampire;
using Content.Server.Antag;
using Content.Server.RPSX.DarkForces.Narsi.Progress;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.Bridges;

public interface IAntagBridge
{
    void ForceMakeCultist(ICommonSession session);

    void ForceMakeCultistLeader(ICommonSession session);

    void ForceMakeRatvarRighteous(ICommonSession session);

    void ForceMakeRatvarRighteous(EntityUid uid);

    void ForceMakeVampire(ICommonSession session);
}

public sealed class StubAntagBridge : IAntagBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string DefaultNarsiRule = "NarsiCult";

    [ValidatePrototypeId<EntityPrototype>]
    private const string DefaultRatvarRule = "Ratvar";

    [ValidatePrototypeId<EntityPrototype>]
    private const string DefaultVampireRule = "Vampire";

    public void ForceMakeCultist(ICommonSession session)
    {
        var antag = _entityManager.System<AntagSelectionSystem>();
        antag.ForceMakeAntag<NarsiRuleComponent>(session, DefaultNarsiRule);
    }

    public void ForceMakeCultistLeader(ICommonSession session)
    {
        if (session.AttachedEntity is not { } entity)
            return;

        var antag = _entityManager.System<AntagSelectionSystem>();
        antag.ForceMakeAntag<NarsiRuleComponent>(session, DefaultNarsiRule);

        if (!_entityManager.HasComponent<NarsiCultistComponent>(entity))
            return;

        var progress = _entityManager.System<NarsiCultProgressSystem>();
        progress.SetNewCultistLeader(entity);
    }

    public void ForceMakeRatvarRighteous(ICommonSession session)
    {
        var antag = _entityManager.System<AntagSelectionSystem>();
        antag.ForceMakeAntag<RatvarRuleComponent>(session, DefaultRatvarRule);
    }

    public void ForceMakeRatvarRighteous(EntityUid uid)
    {
        if (!_entityManager.TryGetComponent<ActorComponent>(uid, out var actor))
            return;

        ForceMakeRatvarRighteous(actor.PlayerSession);
    }

    public void ForceMakeVampire(ICommonSession session)
    {
        var antag = _entityManager.System<AntagSelectionSystem>();
        antag.ForceMakeAntag<VampireRuleComponent>(session, DefaultVampireRule);
    }
}
