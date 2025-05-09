using Content.Shared.Mind;

namespace Content.Server.RPSX.Bridges;

public interface IVampireBridge
{
    void Initialize();
    void MakeVampire(EntityUid mindId, MindComponent mind);

    bool HasVampireRole(EntityUid mindId);

    bool IsVampire(EntityUid uid);
}

public sealed class StubVampireBridge : IVampireBridge
{
    public void Initialize() { }

    public void MakeVampire(EntityUid mindId, MindComponent mind) { }

    public bool HasVampireRole(EntityUid mindId) => false;

    public bool IsVampire(EntityUid uid) => false;
}
