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
            prototypeManager.RegisterIgnore("narsiAbilityPrototype");
            prototypeManager.RegisterIgnore("narsiRitualCategory");
            prototypeManager.RegisterIgnore("narsiRitual");
            prototypeManager.RegisterIgnore("diseaseBlacklistPrototype");
            prototypeManager.RegisterIgnore("disease");
            prototypeManager.RegisterIgnore("diseaseCure");
            prototypeManager.RegisterIgnore("diseaseStage");

        }

        RegisterIoC(useSecrets);
    }

    private void RegisterIoC(bool useSecrets)
    {
        if (useSecrets)
            return;

        IoCManager.RegisterInstance<IAntagBridge>(new StubAntagBridge());
        IoCManager.RegisterInstance<IVampireBridge>(new StubVampireBridge());
        IoCManager.RegisterInstance<ISaintedBridge>(new StubSaintedBridge());
        IoCManager.RegisterInstance<IDiseasesBridge>(new StubDiseasesBridge());
        IoCManager.RegisterInstance<IStatusResponseProvider>(new StubStatusResponseProvider());
        IoCManager.RegisterInstance<ISalaryBridge>(new StubSalaryBridge());
        IoCManager.RegisterInstance<IBankBridge>(new StubBankBridge());

    }
}
