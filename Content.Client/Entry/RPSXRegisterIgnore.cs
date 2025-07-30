using Robust.Shared.ContentPack;
using Content.Client.RPSX.Bridges;

namespace Content.Client.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSX.Client.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSX.Client.dll");

        if (useSecrets)
            return;

        IoCManager.Register<ITargetDollWidgetBridge, StubTargetDollWidgetBridge>();
    }
}
