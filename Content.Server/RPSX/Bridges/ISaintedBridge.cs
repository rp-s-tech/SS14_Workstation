namespace Content.Server.RPSX.Bridges;

public interface ISaintedBridge
{
    bool TryMakeSainted(EntityUid user, EntityUid uid);
}

public sealed class StubSaintedBridge : ISaintedBridge
{
    public bool TryMakeSainted(EntityUid user, EntityUid uid) => false;
}
