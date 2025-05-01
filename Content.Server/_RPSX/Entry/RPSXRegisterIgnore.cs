using Content.Server.Entry;
using Content.Server.RPSX.Bridges;
using Content.Shared.RPSX.Patron;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using Content.Server.RPSX.Sponsors;
using Content.Server.RPSX.Bank.Systems;

namespace Content.Server.RPSX.Entry;

public sealed class RPSXRegisterIgnore
{
    public void RegisterIgnore(IPrototypeManager prototypeManager, IComponentFactory componentFactory, IResourceManager res)
    {
        var useSecrets = res.ContentFileExists("/Content.RPSX.Server.dll") ||
                         res.ContentFileExists("/Assemblies/Content.RPSX.Server.dll");

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
        IoCManager.RegisterInstance<IAntagBridge>(new StubAntagBridge());
        IoCManager.RegisterInstance<IVampireBridge>(new StubVampireBridge());
        IoCManager.RegisterInstance<ISaintedBridge>(new StubSaintedBridge());
        IoCManager.RegisterInstance<IDiseasesBridge>(new StubDiseasesBridge());

        if (useSecrets)
            return;
        IoCManager.RegisterInstance<IStatusResponseProvider>(new StubStatusResponseProvider());
    }
}
