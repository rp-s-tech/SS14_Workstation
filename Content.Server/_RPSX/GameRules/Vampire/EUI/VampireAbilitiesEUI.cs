using Content.Server.RPSX.GameRules.Vampire.Role.Events;
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.RPSX.DarkForces.Vampire;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.GameRules.Vampire.EUI;

[UsedImplicitly]
public sealed class VampireAbilitiesEUI : BaseEui
{
    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not VampireAbilitySelected data)
            return;

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var entityUid = entityManager.GetEntity(data.NetEntity);
        var ev = new VampireAbilitySelectedEvent(data.Action, data.BloodRequired, data.ReplaceId);

        entityManager.EventBus.RaiseLocalEvent(entityUid, ref ev);
        Close();
    }
}
