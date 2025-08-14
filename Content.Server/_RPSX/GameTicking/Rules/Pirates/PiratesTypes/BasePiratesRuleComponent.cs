namespace Content.Server.RPSX.GameTicking.Rules.Pirates.PiratesTypes
{
    [RegisterComponent]
    public abstract partial class BasePiratesRuleComponent : Component
    {
        [DataField]
        public EntityUid Progress = EntityUid.Invalid;

        [DataField]
        public Dictionary<string, int> ObjectivesSilent { get; private set; } = new();

        [DataField]
        public Dictionary<string, int> ObjectivesLoud { get; private set; } = new();
    }
}
