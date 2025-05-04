// Exodus - Stamina Refactor
// Exodus - Stamian Refactor | Remove unnecessary usings
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Movement.Components;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Shared.Damage.Systems;

public abstract partial class SharedStaminaSystem : EntitySystem
{
    // Exodus - Stamian Refactor | Remove unnecessary systems
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;

    // Exodus - Stamina Refactor | Move stamina crit buffer to stamina component

    //public float UniversalStaminaDamageModifier { get; private set; } = 1f;

    public override void Initialize()
    {
        base.Initialize();

        InitializeModifier();
        InitializeResistance();

        // Exodus - Stamian Refactor | Remove ComponentStartup, AfterAutoHandleStateEvent
        SubscribeLocalEvent<StaminaComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<StaminaComponent, DisarmedEvent>(OnDisarmed);
        SubscribeLocalEvent<StaminaComponent, RejuvenateEvent>(OnRejuvenate);

        SubscribeLocalEvent<StaminaDamageOnEmbedComponent, EmbedEvent>(OnProjectileEmbed);

        SubscribeLocalEvent<StaminaDamageOnCollideComponent, ProjectileHitEvent>(OnProjectileHit);
        SubscribeLocalEvent<StaminaDamageOnCollideComponent, ThrowDoHitEvent>(OnThrowHit);

        SubscribeLocalEvent<StaminaDamageOnHitComponent, MeleeHitEvent>(OnMeleeHit);

        // Subs.CVar(_config, CCVars.PlaytestStaminaDamageModifier, value => UniversalStaminaDamageModifier = value, true);

        SubscribeLocalEvent<StaminaComponent, StaminaDamageModifyEvent>(OnStaminaDamage);  // Exodus - Stamina Refactor
        SubscribeLocalEvent<StaminaComponent, RefreshDecayEvent>(OnRefreshDecay);  // Exodus - Stamina Refactor
    }

    private void OnShutdown(EntityUid uid, StaminaComponent component, ComponentShutdown args)
    {
        if (MetaData(uid).EntityLifeStage < EntityLifeStage.Terminating)
        {
            RemCompDeferred<ActiveStaminaComponent>(uid);
        }
        // Exodus - Stamina Refactor | Remove stamina alert
    }

    // Exodus - Stamina Refector | Remove OnStartup
    // Exodus - Stamina Refector | Remove GetStaminaDamage, get it from component

    // Exodus - Rush | Stamina Armor when walking
    private void OnStaminaDamage(Entity<StaminaComponent> entity, ref StaminaDamageModifyEvent ev)
    {
        if (!TryComp<InputMoverComponent>(entity, out var mover) ||
            !mover.Walking)
            return;

        ev.Damage *= entity.Comp.DamageWalkingModify;
    }
    // Exodus - end

    private void OnRejuvenate(EntityUid uid, StaminaComponent component, RejuvenateEvent args)
    {
        // Exodus - Stamina Refector | All logic move to StaminaRecover
        if (component.StaminaDamage >= 0)
            StaminaRecover(uid, component);
    }

    private void OnDisarmed(EntityUid uid, StaminaComponent component, ref DisarmedEvent args)
    {
        if (args.Handled)
            return;

        if (IsStunned(component)) // Exodus - Stamina Refector | IsStunned instead of Critical
            return;

        var damage = args.PushProbability * component.CritThreshold;
        TakeStaminaDamage(uid, damage, component, source: args.Source);

        args.PopupPrefix = "disarm-action-shove-";
        args.IsStunned = IsStunned(component);  // Exodus - Stamina Refector | IsStunned instead of Critical

        args.Handled = true;
    }

    private void OnMeleeHit(EntityUid uid, StaminaDamageOnHitComponent component, MeleeHitEvent args)
    {
        if (!args.IsHit ||
            !args.HitEntities.Any() ||
            component.Damage <= 0f) // Exodus - Stamina Refector | Remove brackets
            return;

        var ev = new StaminaDamageOnHitAttemptEvent();
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        var stamQuery = GetEntityQuery<StaminaComponent>();
        var toHit = new List<(EntityUid Entity, StaminaComponent Component)>();

        // Split stamina damage between all eligible targets.
        foreach (var ent in args.HitEntities)
        {
            if (!stamQuery.TryGetComponent(ent, out var stam))
                continue;

            toHit.Add((ent, stam));
        }

        var hitEvent = new StaminaMeleeHitEvent(toHit);
        RaiseLocalEvent(uid, hitEvent);

        if (hitEvent.Handled)
            return;

        var damage = component.Damage;
        damage *= hitEvent.Multiplier;
        damage += hitEvent.FlatModifier;

        foreach (var (ent, comp) in toHit)
        {
            var modifyEv = new StaminaDamageModifyEvent(damage);
            RaiseLocalEvent(ent, ref modifyEv);

            TakeStaminaDamage(ent, modifyEv.Damage / toHit.Count, comp, source: args.User, with: args.Weapon, sound: component.Sound);
        }
    }

    private void OnProjectileHit(EntityUid uid, StaminaDamageOnCollideComponent component, ref ProjectileHitEvent args)
    {
        OnCollide(uid, component, args.Target);
    }

    private void OnProjectileEmbed(EntityUid uid, StaminaDamageOnEmbedComponent component, ref EmbedEvent args)
    {
        if (!TryComp<StaminaComponent>(args.Embedded, out var stamina))
            return;

        TakeStaminaDamage(args.Embedded, component.Damage, stamina, source: uid);
    }

    private void OnThrowHit(EntityUid uid, StaminaDamageOnCollideComponent component, ThrowDoHitEvent args)
    {
        OnCollide(uid, component, args.Target);
    }

    private void OnCollide(EntityUid uid, StaminaDamageOnCollideComponent component, EntityUid target)
    {
        // you can't inflict stamina damage on things with no stamina component
        // this prevents stun batons from using up charges when throwing it at lockers or lights
        if (!HasComp<StaminaComponent>(target))
            return;

        var ev = new StaminaDamageOnHitAttemptEvent();
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        var modifyEv = new StaminaDamageModifyEvent(component.Damage);
        RaiseLocalEvent(target, ref modifyEv);

        TakeStaminaDamage(target, modifyEv.Damage, source: uid, sound: component.Sound);
    }

    // Exodus - Stamina Refector | Remove SetStaminaAlert

    /// <summary>
    /// Tries to take stamina damage without raising the entity over the crit threshold.
    /// </summary>
    public bool TryTakeStamina(EntityUid uid, float value, StaminaComponent? component = null, EntityUid? source = null, EntityUid? with = null)
    {
        // Something that has no Stamina component automatically passes stamina checks
        if (!Resolve(uid, ref component, false))
            return true;

        // Exodus - Stamina Refector | Correct logic
        if (value > 0 && (component.StaminaDamage + value >= component.CritThreshold || IsStunned(component)))
            return false;

        TakeStaminaDamage(uid, value, component, source, with, visual: false);
        return true;
    }

    public void TakeStaminaDamage(EntityUid uid, float value, StaminaComponent? component = null,
        EntityUid? source = null, EntityUid? with = null, bool visual = true, SoundSpecifier? sound = null, bool ignoreResist = false)
    {
        if (!Resolve(uid, ref component, false))
            return;

        var ev = new BeforeStaminaDamageEvent(value);
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        // Allow stamina resistance to be applied.
        if (!ignoreResist)
        {
            value = ev.Value;
        }

        // value = UniversalStaminaDamageModifier * value;

        // Exdodus - Stamina Refeactor | Sepatate functions
        component.StaminaDamage += value;
        UpdateStamina(uid, component);
        RefreshDecay(uid, component);

        // Exodus - Stamina Refector | Remove slowdown when damaged catched
        // Exodus - Stamina Refector | Remove stamina alert

        if (value > 0)
            component.LastDamage = _timing.CurTime;

        EnsureComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);

        if (value <= 0)
            return;

        if (source != null)
        {
            _adminLogger.Add(LogType.Stamina, $"{ToPrettyString(source.Value):user} caused {value} stamina damage to {ToPrettyString(uid):target}{(with != null ? $" using {ToPrettyString(with.Value):using}" : "")}");
        }
        else
        {
            _adminLogger.Add(LogType.Stamina, $"{ToPrettyString(uid):target} took {value} stamina damage");
        }

        if (visual)
        {
            _color.RaiseEffect(Color.Aqua, new List<EntityUid>() { uid }, Filter.Pvs(uid, entityManager: EntityManager));
        }

        if (_net.IsServer)
        {
            _audio.PlayPvs(sound, uid);
        }
    }

    // Exodus - Stamina Refector - Start | Separete logic
    private void UpdateStamina(EntityUid uid, StaminaComponent component)
    {
        // Have we already reached the point of max stamina damage?

        component.StaminaDamage = MathF.Max(0f, component.StaminaDamage);
        component.StaminaDamage = MathF.Min(component.CritThreshold, component.StaminaDamage);

        // Exodus - Remove Slow down when stamina damage

        component.IsInDanger = component.IsInDanger ?
            component.StaminaDamage >= component.ResetDangerThreshold :
            component.StaminaDamage >= component.SetDangerThreshold;

        if (component.StaminaDamage == component.CritThreshold &&
            component.LastStun + component.StunInterval + component.StunTime < _timing.CurTime)
            StaminaStun(uid, component);
    }
    // Exodus - End

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        var stamQuery = GetEntityQuery<StaminaComponent>();
        var query = EntityQueryEnumerator<ActiveStaminaComponent>();
        var curTime = _timing.CurTime;

        while (query.MoveNext(out var uid, out _))
        {
            // Just in case we have active but not stamina we'll check and account for it.
            if (!stamQuery.TryGetComponent(uid, out var comp) ||
                comp.StaminaDamage <= 0f && comp.Decay > 0) // Exodus - Stamina Refector | Correct logic
            {
                RemComp<ActiveStaminaComponent>(uid);
                continue;
            }

            // Exodus - Stamina Refactor | Separate function

            // Decay
            RefreshDecay(uid, comp);
            var totalDecay = comp.Decay * frameTime;
            TryTakeStamina(uid, -totalDecay, comp);
            //

            UpdateStamina(uid, comp);
            Dirty(uid, comp);
        }
    }

    // Exodus - Stamina Refactor - Start | Replace EnterStamCrit
    private void StaminaStun(EntityUid uid, StaminaComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // To make the difference between a stun and a stamcrit clear
        // TODO: Mask?

        component.LastStun = _timing.CurTime;
        _stunSystem.TryParalyze(uid, component.StunTime, true);

        EnsureComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);
        _adminLogger.Add(LogType.Stamina, LogImpact.Medium, $"{ToPrettyString(uid):user} entered stamina stun");
    }
    // Exodus - End

    // Exodus - Stamina Refactor - Start | Replace ExitStamCrit
    public void StaminaRecover(EntityUid uid, StaminaComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.StaminaDamage = 0;
        RefreshDecay(uid);

        RemComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);
        _adminLogger.Add(LogType.Stamina, LogImpact.Low, $"{ToPrettyString(uid):user} recovered from stamina crit");
    }
    // Exodus - End

    // Exodus - Stamina Refactor - Start | Separate function
    public bool IsStunned(EntityUid uid)
    {
        return TryComp<StaminaComponent>(uid, out var stamina) && IsStunned(stamina);
    }

    public bool IsStunned(StaminaComponent component)
    {
        return component.LastStun + component.StunTime > _timing.CurTime;
    }
    // Exodus - End
}

/// <summary>
///     Raised before stamina damage is dealt to allow other systems to cancel it.
/// </summary>
[ByRefEvent]
public record struct BeforeStaminaDamageEvent(float Value, bool Cancelled = false);
