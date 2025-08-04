using Content.Server.Entry;
using Robust.Shared.ContentPack;

namespace Content.Server.RPSX.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IComponentFactory componentFactory, IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSX.Server.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSX.Server.dll");

        if (!useSecrets)
        {
            componentFactory.RegisterIgnore(IgnoredSecretComponents.List);
        }
    }
}
