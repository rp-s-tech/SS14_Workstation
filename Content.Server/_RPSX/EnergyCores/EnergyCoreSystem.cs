using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.RPSX.EnergyCores;
using Robust.Shared.Timing;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Server.GameObjects;
using Content.Server.Power.Components;
using Content.Server.Administration.Logs;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Content.Shared.Database;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Atmos.Piping.Components;
using Content.Shared.Atmos;
using Content.Server.NodeContainer.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Gravity;
using Content.Server.Gravity;
using Content.Server.Shuttles.Systems;
using Content.Server.Shuttles.Components;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.UserInterface;

namespace Content.Server.RPSX.EnergyCores;

public sealed partial class EnergyCoreSystem : EntitySystem
{
    [Dependency] private readonly GasVentScrubberSystem _scrubberSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IEntityManager _e = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly GravitySystem _gravitySystem = default!;
    [Dependency] private readonly ThrusterSystem _thrusterSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    private EntityQuery<PowerSupplierComponent> _recQuery;
    private TimeSpan _nextTickCore = TimeSpan.FromSeconds(1);

    public override void Initialize()
    {
        SubscribeLocalEvent<EnergyCoreComponent, MapInitEvent>(OnMapInit);
        _recQuery = GetEntityQuery<PowerSupplierComponent>();
        SubscribeLocalEvent<HeatFreezingCoreComponent, AtmosDeviceUpdateEvent>(OnDeviceUpdated);
        SubscribeLocalEvent<EnergyCoreComponent, TogglePowerDoAfterEvent>(TogglePowerDoAfter);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, NewLinkEvent>(OnNewLink);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, UserOpenActivatableUIAttemptEvent>(OnTryOpenUI);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, EnergyCoreConsoleIsOnMessage>(OnPowerToggled);
        SubscribeLocalEvent<EnergyCoreComponent, EntParentChangedMessage>(OnParentChanged);
    }
    private void OnMapInit(EntityUid uid, EnergyCoreComponent component, MapInitEvent args)
    {
        component.ForceDisabled = true;
        TogglePowerDiscrete(uid, core: component);
        component.TimeOfLife = 0;
        if (!TryComp(uid, out HeatFreezingCoreComponent? heatFreezing)) return;
        heatFreezing.FilterGases.Add(heatFreezing.AbsorbGas);
    }

    private void OnDeviceUpdated(EntityUid uid, HeatFreezingCoreComponent component, ref AtmosDeviceUpdateEvent args)
    {
        var timeDelta = args.dt;
        // If we are on top of a connector port, empty into it.
        if (!_nodeContainer.TryGetNode(uid, component.PortName, out PipeNode? portableNode))
            return;
        if (args.Grid is not { } grid)
            return;
        if (!TryComp(uid, out EnergyCoreComponent? core)) return;
        var position = _transformSystem.GetGridTilePositionOrDefault(uid);
        var environment = _atmosphereSystem.GetTileMixture(grid, args.Map, position, true);
        // widenet
        var enumerator = _atmosphereSystem.GetAdjacentTileMixtures(grid, position, false, true);
        while (enumerator.MoveNext(out var adjacent))
        {
            Scrub(timeDelta, portableNode, adjacent, component);
            if (core.TimeOfLife < 1000)
                core.TimeOfLife += portableNode.Air.GetMoles(component.AbsorbGas) * core.SecPerMoles;
            else
                core.TimeOfLife = 1000;
            portableNode.Air.Clear();
            if (environment != null && core.Working && core.Size == 2)
                _atmosphereSystem.AddHeat(environment, 300);
            else if (environment != null && core.Working && core.Size == 3)
                _atmosphereSystem.AddHeat(environment, 600);
        }
        if (core.TimeOfLife > 0 && core.ForceDisabled)
            core.ForceDisabled = false;
        if (environment != null && environment.Temperature >= 750)
            OverHeating(core);
    }


    private bool Scrub(float timeDelta, PipeNode scrubber, GasMixture tile, HeatFreezingCoreComponent target)
    {
        if (tile.Temperature > target.FilterTemperature) return false;
        return _scrubberSystem.Scrub(timeDelta, target.TransferRate * _atmosphereSystem.PumpSpeedup(), ScrubberPumpDirection.Scrubbing, target.FilterGases, tile, scrubber.Air);
    }
    // private void Pump(GasMixture? enviroment, PipeNode pipe, HeatFreezingCoreComponent target)
    // {
    //     if (enviroment == null || pipe == null) return;
    //     _atmosphereSystem.Merge(enviroment, pipe.Air.Remove(target.TransferRate * _atmosphereSystem.PumpSpeedup()));
    // }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_timing.CurTime > _nextTickCore)
        {
            EnergyCoreTick();
            _nextTickCore += TimeSpan.FromSeconds(1);
        }
    }

    private void OverHeating(EnergyCoreComponent component)
    {
        if (!component.Overheat) component.Overheat = true;
        _damageable.TryChangeDamage(component.Owner, component.Damage, true);

        var environment = _atmosphereSystem.GetTileMixture(component.Owner);
        if (environment != null)
            environment.Temperature += component.Heating;
    }
    private void Absorb(EnergyCoreComponent component, PipeNode air)
    {
        if (component.Overheat && component.TimeOfLife > 0)
        {
            ForceTurnOff(component);
        }
    }

    private void ForceTurnOff(EnergyCoreComponent component)
    {
        component.Overheat = false;
        component.ForceDisabled = true;
        if (component.Working)
            TogglePower(component.Owner);
    }

    private void Working(EnergyCoreComponent component, PipeNode air)
    {
        Absorb(component, air);
        if (component.Working && !component.ForceDisabled)
        {
            if (component.TimeOfLife > component.LifeAfterOverheat)
            {
                component.TimeOfLife -= 1;
                if (component.TimeOfLife <= 0 && !component.isUndead)
                    OverHeating(component);
            }
            else
            {
                ForceTurnOff(component);
            }
        }
    }
    private void EnergyCoreTick()
    {
        var query = EntityQueryEnumerator<EnergyCoreComponent>();
        while (query.MoveNext(out var ent, out var target))
        {
            if (!TryComp(target.Owner, out DamageableComponent? damage)) return;
            var energyCore = target.Owner;
            var timeOfLife = target.TimeOfLife;
            var isOn = target.Working;
            var console = target.EnergyCoreConsoleEntity;
            var curDamage = damage.TotalDamage.Float();
            if (_timing.CurTime > target.NextTick)
            {
                if (!TryComp<NodeContainerComponent>(target.Owner, out var component))
                    continue;
                if (!TryComp<HeatFreezingCoreComponent>(target.Owner, out var heatfreeze))
                    continue;
                if (!_nodeContainer.TryGetNode(target.Owner, heatfreeze.PortName, out PipeNode? cur))
                {
                    continue;
                }
                Working(target, cur);
            }
            if (console is not EntityUid entity) return;
            _ui.SetUiState(entity, EnergyCoreConsoleUiKey.Key, new EnergyCoreConsoleUpdateState(GetNetEntity(energyCore), timeOfLife, isOn, curDamage));
        }
    }

    public void TogglePower(EntityUid uid, bool playSwitchSound = true, EnergyCoreComponent? core = null, EntityUid? user = null)
    {
        if (core == null) if (!TryComp(uid, out core)) return;
        if (core.ForceDisabled) return;
        if (!TryComp(uid, out ApcPowerReceiverComponent? receiver)) return;
        EnergyCoreState dataForSet;
        if (receiver.PowerDisabled)
            dataForSet = EnergyCoreState.Enabling;
        else
            dataForSet = EnergyCoreState.Disabling;
        _appearance.SetData(uid, EnergyCoreVisualLayers.IsOn, dataForSet);
        var time = receiver.PowerDisabled ? core.EnablingLenght : core.DisablingLenght;
        _doAfterSystem.TryStartDoAfter(new DoAfterArgs(_e, uid, TimeSpan.FromSeconds(time), new TogglePowerDoAfterEvent(_e.GetNetEntity(user)), uid, target: uid, used: uid));
    }
    private void TogglePowerDoAfter(EntityUid uid, EnergyCoreComponent component, TogglePowerDoAfterEvent args)
    {
        TogglePowerDiscrete(uid, core: component, user: _e.GetEntity(args.Initer));
    }

    private bool TogglePowerDiscrete(EntityUid uid, bool playSwitchSound = true, EnergyCoreComponent? core = null, EntityUid? user = null)
    {
        if (core == null) return true;
        if (!TryComp(uid, out PowerSupplierComponent? supplier)) return true;
        if (!TryComp(uid, out ApcPowerReceiverComponent? receiver)) return true;

        supplier.Enabled = !supplier.Enabled;

        if (supplier.Enabled)
            supplier.MaxSupply = core.BaseSupply;
        else
            supplier.MaxSupply = 0;

        if (!receiver.NeedsPower)
        {
            receiver.PowerDisabled = false;
            return true;
        }
        receiver.PowerDisabled = !receiver.PowerDisabled;

        if (user != null)
            _adminLogger.Add(LogType.Action, LogImpact.Low, $"{ToPrettyString(user.Value):player} hit power button on {ToPrettyString(uid)}, it's now {(!supplier.Enabled ? "on" : "off")}");

        if (playSwitchSound)
        {
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg"), uid,
                AudioParams.Default.WithVolume(-2f));
        }
        var dataForSet = !receiver.PowerDisabled ? EnergyCoreState.Enabled : EnergyCoreState.Disabled;
        _appearance.SetData(uid, EnergyCoreVisualLayers.IsOn, dataForSet);
        core.Working = !receiver.PowerDisabled;

        if (TryComp(uid, out TransformComponent? xform) &&
            TryComp<GravityComponent>(xform.ParentUid, out var gravity))
        {
            if (core.Working)
            {
                _gravitySystem.EnableGravity(xform.ParentUid, gravity);
            }
            else
            {
                _gravitySystem.RefreshGravity(xform.ParentUid, gravity);
            }
        }
        if (!TryComp(uid, out ThrusterComponent? thruster)) return true;
        if (!TryComp(uid, out TransformComponent? xForm)) return true;
        if (core.Working)
            _thrusterSystem.EnableThruster(uid, thruster, xForm);
        else
            _thrusterSystem.DisableThruster(uid, thruster, xForm);
        return !supplier.Enabled && !receiver.PowerDisabled; // i.e. PowerEnabled
    }

    private void OnNewLink(EntityUid uid, EnergyCoreConsoleComponent component, NewLinkEvent args)
    {
        if (!TryComp<EnergyCoreComponent>(args.Sink, out var analyzer))
            return;

        component.EnergyCoreEntity = args.Sink;
        analyzer.EnergyCoreConsoleEntity = uid;
    }

    private void OnTryOpenUI(EntityUid console, EnergyCoreConsoleComponent component, UserOpenActivatableUIAttemptEvent args)
    {
        if (component.EnergyCoreEntity is not EntityUid entity)
        {
            args.Cancel();
        }
    }
    private void OnPowerToggled(EntityUid uid, EnergyCoreConsoleComponent component, EnergyCoreConsoleIsOnMessage args)
    {
        if (!TryComp(component.EnergyCoreEntity, out EnergyCoreComponent? core)) return;
        TogglePower(core.Owner);
    }
    private void OnParentChanged(EntityUid uid, EnergyCoreComponent component, ref EntParentChangedMessage args)
    {
        if (TryComp(args.OldParent, out GravityComponent? gravity))
        {
            _gravitySystem.RefreshGravity(args.OldParent.Value, gravity);
        }
    }
}
