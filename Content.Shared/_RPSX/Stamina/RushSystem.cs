using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Movement.Systems;
using Robust.Shared.Timing;

using Content.Shared.RPSX.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;

namespace Content.Shared.RPSX.Stamina;

public sealed partial class RushSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;

    private EntityQuery<InputMoverComponent> _inputQuery = default!;
    private EntityQuery<RushComponent> _rushQuery = default!;
    private EntityQuery<StaminaComponent> _staminaQuery = default!;


    public override void Initialize()
    {
        base.Initialize();

        _inputQuery = GetEntityQuery<InputMoverComponent>();
        _rushQuery = GetEntityQuery<RushComponent>();
        _staminaQuery = GetEntityQuery<StaminaComponent>();

        SubscribeLocalEvent<RushComponent, StaminaDamageModifyEvent>(OnStaminaDamage);
    }

    private void OnStaminaDamage(Entity<RushComponent> entity, ref StaminaDamageModifyEvent ev)
    {
        if (!_inputQuery.TryComp(entity, out var mover) ||
            !mover.Rushing)
            return;

        ev.Damage *= entity.Comp.StaminaDamageModify;
    }

    public float GetRushModify(EntityUid uid)
    {
        if (_rushQuery.TryComp(uid, out var rush))
            return rush.RushModify;
        else
            return 1;
    }

    public (float RushFraction, float sprintFraction) GetRushFracAndUdpateStamina(EntityUid uid, float fraction)
    {
        if (!_timing.InSimulation)
            return (0, 1);

        if (!_rushQuery.TryComp(uid, out var rush) ||
            !_staminaQuery.TryComp(uid, out var stamina))
            return (0, 1);

        float rushFrac = 0;
        float sprintFrac = 0;

        var period = (float)_timing.TickPeriod.TotalSeconds * fraction;
        var remainTime = (float)_timing.TickPeriod.TotalSeconds * fraction;
        while (remainTime > 0)
        {
            if (!stamina.IsInDanger)
            {
                if (stamina.StaminaDamage
                    - stamina.Decay * remainTime
                    + rush.StaminaDrain * remainTime > stamina.DangerThreshold)
                {
                    _stamina.TakeStaminaDamage(uid, stamina.DangerThreshold - stamina.StaminaDamage, stamina, visual: false);

                    var rushTime = (stamina.DangerThreshold - stamina.StaminaDamage) / (rush.StaminaDrain - stamina.Decay);
                    rushFrac += rushTime / period;
                    remainTime -= rushTime;
                }
                else
                {
                    _stamina.TakeStaminaDamage(uid, remainTime * rush.StaminaDrain, stamina, visual: false);

                    rushFrac += remainTime / period;
                    remainTime = 0;
                }
            }
            else
            {
                if (stamina.StaminaDamage
                - stamina.Decay * remainTime < stamina.DangerThreshold)
                {
                    var sprintTime = (stamina.StaminaDamage - stamina.DangerThreshold) / stamina.Decay;
                    sprintFrac += sprintTime / period;
                    remainTime -= sprintTime;
                }
                else
                {
                    sprintFrac += remainTime / period;
                    remainTime = 0;
                }
            }

        }

        return (rushFrac, sprintFrac);
    }
}
