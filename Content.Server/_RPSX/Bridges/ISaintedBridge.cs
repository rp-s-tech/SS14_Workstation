using Content.Server.RPSX.DarkForces.Saint.Saintable;

namespace Content.Server.RPSX.Bridges;

public interface ISaintedBridge
{
    bool TryMakeSainted(EntityUid user, EntityUid uid);
}

public sealed class StubSaintedBridge : ISaintedBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public bool TryMakeSainted(EntityUid user, EntityUid uid)
    {
        return _entityManager.System<SaintedSystem>().TryMakeSainted(user, uid);
    }
}
