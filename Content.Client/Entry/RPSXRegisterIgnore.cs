using Content.Shared.RPSX.Bridges;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;

namespace Content.Client.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IPrototypeManager prototypeManager, IComponentFactory componentFactory, IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSX.Client.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSX.Client.dll");

        if (useSecrets)
            return;

        RegisterIoC();
    }

    public void RegisterIoC()
    {
        IoCManager.RegisterInstance<IBankBridge>(new StubBankBridge());
    }
}
