using Robust.Shared.Player;

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
    public void ForceMakeCultist(ICommonSession session) { }

    public void ForceMakeCultistLeader(ICommonSession session) { }

    public void ForceMakeRatvarRighteous(ICommonSession session) { }

    public void ForceMakeRatvarRighteous(EntityUid uid) { }

    public void ForceMakeVampire(ICommonSession session) { }
}
