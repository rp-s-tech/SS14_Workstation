using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using Content.Client.RPSX.Bridges;

namespace Content.Client.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IPrototypeManager prototypeManager, IComponentFactory componentFactory, IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSX.Client.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSX.Client.dll");

        if (useSecrets)
            return;

        IoCManager.RegisterInstance<ITargetDollWidgetBridge>(new StubTargetDollWidgetBridge());
    }
}
