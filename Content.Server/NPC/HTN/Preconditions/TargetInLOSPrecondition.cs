using Content.Server.NPC.Systems; // Exodus-TurretsImprovement-Start

namespace Content.Server.NPC.HTN.Preconditions;

public sealed partial class TargetInLOSPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    private NPCCombatSystem _npcCombat = default!; // Exodus-TurretsImprovement-Start

    [DataField("targetKey")]
    public string TargetKey = "Target";

    [DataField("rangeKey")]
    public string RangeKey = "RangeKey";

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _npcCombat = sysManager.GetEntitySystem<NPCCombatSystem>(); // Exodus-TurretsImprovement-Start
    }

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        if (!blackboard.TryGetValue<EntityUid>(TargetKey, out var target, _entManager))
            return false;

        var range = blackboard.GetValueOrDefault<float>(RangeKey, _entManager);

        return _npcCombat.IsEnemyInLOS(owner, target, range); // Exodus-TurretsImprovement-Start
    }
}
