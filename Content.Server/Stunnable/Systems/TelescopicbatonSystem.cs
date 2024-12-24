using Content.Server.Power.EntitySystems;
using Content.Server.Stunnable.Components;
using Content.Shared.Audio;
using Content.Shared.Damage.Events;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Toggleable;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Stunnable.Systems
{
    public sealed class TelescopicbatonSystem : SharedTelescopicbatonSystem
    {
        [Dependency] private readonly SharedItemSystem _item = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly RiggableSystem _riggableSystem = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly BatterySystem _battery = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<TelescopicbatonComponent, UseInHandEvent>(OnUseInHand);
            SubscribeLocalEvent<TelescopicbatonComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<TelescopicbatonComponent, StaminaDamageOnHitAttemptEvent>(OnStaminaHitAttempt);
        }

        private void OnStaminaHitAttempt(EntityUid uid, TelescopicbatonComponent component, ref StaminaDamageOnHitAttemptEvent args)
        {
            if (!component.Activated)
            {
                args.Cancelled = true;
                return;
            }
        }

        private void OnUseInHand(EntityUid uid, TelescopicbatonComponent comp, UseInHandEvent args)
        {
            if (comp.Activated)
            {
                TurnOff(uid, comp);
            }
            else
            {
                TurnOn(uid, comp, args.User);
            }
        }

        private void OnExamined(EntityUid uid, TelescopicbatonComponent comp, ExaminedEvent args)
        {
            var msg = comp.Activated
                ? Loc.GetString("comp-telescopicbaton-examined-on")
                : Loc.GetString("comp-telescopicbaton-examined-off");
            args.PushMarkup(msg);
        }

        private void TurnOff(EntityUid uid, TelescopicbatonComponent comp)
        {
            if (!comp.Activated)
                return;

            if (TryComp<AppearanceComponent>(uid, out var appearance) &&
                TryComp<ItemComponent>(uid, out var item))
            {
                _item.SetHeldPrefix(uid, "off", component: item);
                _appearance.SetData(uid, ToggleVisuals.Toggled, false, appearance);
            }

            _audio.PlayPvs(comp.SparksSound, uid, AudioHelpers.WithVariation(0.25f));

            comp.Activated = false;
            Dirty(uid, comp);
        }

        private void TurnOn(EntityUid uid, TelescopicbatonComponent comp, EntityUid user)
        {
            if (comp.Activated)
                return;

            if (EntityManager.TryGetComponent<AppearanceComponent>(uid, out var appearance) &&
                EntityManager.TryGetComponent<ItemComponent>(uid, out var item))
            {
                _item.SetHeldPrefix(uid, "on", component: item);
                _appearance.SetData(uid, ToggleVisuals.Toggled, true, appearance);
            }

            _audio.PlayPvs(comp.SparksSound, uid, AudioHelpers.WithVariation(0.25f));
            comp.Activated = true;
            Dirty(uid, comp);
        }
    }
}
