using Content.Shared.Cargo.Prototypes;
using Content.Shared.GameTicking;

namespace Content.Server.RPSX.Bridges;

//Little bridge to communicate from secrets
public sealed class CargoBridge : EntitySystem
{
    private List<CargoProductPrototype> advancedPrototypes = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
    }

    private void OnRoundEnded(RoundEndedEvent ev)
    {
        advancedPrototypes.Clear();
    }

    public void AddAdvancedPrototypes(List<CargoProductPrototype> newPrototypes)
    {
        advancedPrototypes.AddRange(newPrototypes);
    }

    public List<CargoProductPrototype> GetAdvancedPrototypes()
    {
        return advancedPrototypes;
    }
}
