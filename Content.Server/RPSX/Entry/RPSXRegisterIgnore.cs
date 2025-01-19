using Content.Server.Entry;
using Content.Server.RPSX.Bridges;
using Content.Shared.RPSX.Patron;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using Content.Server.RPSX.Sponsors;

namespace Content.Server.RPSX.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IPrototypeManager prototypeManager, IComponentFactory componentFactory, IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSXServer.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSXServer.dll");

        if (!useSecrets)
        {
            componentFactory.RegisterIgnore(IgnoredSecretComponents.List);

            prototypeManager.RegisterIgnore("stationGoal");

        }

        RegisterIoC(useSecrets);
    }

    private void RegisterIoC(bool useSecrets)
    {
        if (useSecrets)
            return;

        IoCManager.RegisterInstance<IStatusResponseProvider>(new StubStatusResponseProvider());
        IoCManager.RegisterInstance<ISalaryBridge>(new StubSalaryBridge());
        IoCManager.RegisterInstance<IBankBridge>(new StubBankBridge());

    }
}
