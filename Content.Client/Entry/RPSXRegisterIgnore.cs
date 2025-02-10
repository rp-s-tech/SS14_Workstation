using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;

namespace Content.Client.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IPrototypeManager prototypeManager, IComponentFactory componentFactory, IResourceManager res)
    {
        //Do nothing at this time
    }

    public void RegisterIoC()
    {
        var res = IoCManager.Resolve<IResourceManager>();
        if (UseSecrets(res))
            return;

    }

    private bool UseSecrets(IResourceManager res)
    {
        return res.ContentFileExists("/Content.RPSX.Client.dll") ||
               res.ContentFileExists("/Assemblies/Content.RPSX.Client.dll");
    }
}
