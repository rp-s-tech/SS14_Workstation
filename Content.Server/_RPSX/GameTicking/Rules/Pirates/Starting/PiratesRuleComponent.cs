namespace Content.Server.RPSX.GameTicking.Rules.Pirates.Starting;

[RegisterComponent]
[Access(typeof(PiratesRuleSystem))]
public sealed partial class PiratesRuleComponent : Component
{
    /// <summary>
    /// The gamerules that get added by secret.
    /// </summary>
    [DataField("additionalGameRules")]
    public HashSet<EntityUid> AdditionalGameRules = new();
}
