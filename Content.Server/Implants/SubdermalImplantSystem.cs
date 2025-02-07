using Content.Server.Cuffs;
using Content.Server.Forensics;
using Content.Server.Humanoid;
using Content.Server.Implants.Components;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Cuffs.Components;
using Content.Shared.Forensics;
using Content.Shared.Humanoid;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
// using Content.Shared.Physics; | RPSX - RandomTeleport Refactor
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Robust.Shared.Audio.Systems;
// using Robust.Shared.Map; | RPSX - RandomTeleport Refactor
// using Robust.Shared.Physics; | RPSX - RandomTeleport Refactor
// using Robust.Shared.Physics.Components; | RPSX - RandomTeleport Refactor
// using Robust.Shared.Random; | RPSX - RandomTeleport Refactor
// using System.Numerics; | RPSX - RandomTeleport Refactor
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Server.IdentityManagement;
using Content.Server.DetailExaminable;
using Content.Shared.Store.Components;
// using Robust.Shared.Collections; | RPSX - RandomTeleport Refactor
// using Robust.Shared.Map.Components; | RPSX - RandomTeleport Refactor
using Content.Server.RPSX.RandomTeleport; // RPSX - RandomTeleport Refactor | add using


namespace Content.Server.Implants;

public sealed class SubdermalImplantSystem : SharedSubdermalImplantSystem
{
    [Dependency] private readonly CuffableSystem _cuffable = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidAppearance = default!;
    // [Dependency] private readonly IRobustRandom _random = default!; | RPSX - RandomTeleport Refactor
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly ForensicsSystem _forensicsSystem = default!;
    [Dependency] private readonly PullingSystem _pullingSystem = default!;
    // [Dependency] private readonly EntityLookupSystem _lookupSystem = default!; | RPSX - RandomTeleport Refactor
    // [Dependency] private readonly SharedMapSystem _mapSystem = default!; | RPSX - RandomTeleport Refactor
    [Dependency] private readonly IdentitySystem _identity = default!;
    [Dependency] private readonly RandomTeleportSystem _randomTeleport = default!; // RPSX - RandomTeleport Refactor | add dependency

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SubdermalImplantComponent, UseFreedomImplantEvent>(OnFreedomImplant);
        SubscribeLocalEvent<StoreComponent, ImplantRelayEvent<AfterInteractUsingEvent>>(OnStoreRelay);
        SubscribeLocalEvent<SubdermalImplantComponent, ActivateImplantEvent>(OnActivateImplantEvent);
        SubscribeLocalEvent<SubdermalImplantComponent, UseScramImplantEvent>(OnScramImplant);
        SubscribeLocalEvent<SubdermalImplantComponent, UseDnaScramblerImplantEvent>(OnDnaScramblerImplant);

    }

    private void OnStoreRelay(EntityUid uid, StoreComponent store, ImplantRelayEvent<AfterInteractUsingEvent> implantRelay)
    {
        var args = implantRelay.Event;

        if (args.Handled)
            return;

        // can only insert into yourself to prevent uplink checking with renault
        if (args.Target != args.User)
            return;

        if (!TryComp<CurrencyComponent>(args.Used, out var currency))
            return;

        // same as store code, but message is only shown to yourself
        if (!_store.TryAddCurrency((args.Used, currency), (uid, store)))
            return;

        args.Handled = true;
        var msg = Loc.GetString("store-currency-inserted-implant", ("used", args.Used));
        _popup.PopupEntity(msg, args.User, args.User);
    }

    private void OnFreedomImplant(EntityUid uid, SubdermalImplantComponent component, UseFreedomImplantEvent args)
    {
        if (!TryComp<CuffableComponent>(component.ImplantedEntity, out var cuffs) || cuffs.Container.ContainedEntities.Count < 1)
            return;

        _cuffable.Uncuff(component.ImplantedEntity.Value, cuffs.LastAddedCuffs, cuffs.LastAddedCuffs);
        args.Handled = true;
    }

    private void OnActivateImplantEvent(EntityUid uid, SubdermalImplantComponent component, ActivateImplantEvent args)
    {
        args.Handled = true;
    }

    // RPSX - RandomTeleport Refactor - start
    private void OnScramImplant(EntityUid uid, SubdermalImplantComponent component, UseScramImplantEvent args)
    {
        if (component.ImplantedEntity is not { } ent)
            return;

        if (!TryComp<ScramImplantComponent>(uid, out var implant))
            return;

        // We need stop the user from being pulled so they don't just get "attached" with whoever is pulling them.
        // This can for example happen when the user is cuffed and being pulled.
        if (TryComp<PullableComponent>(ent, out var pull) && _pullingSystem.IsPulled(ent, pull))
            _pullingSystem.TryStopPull(ent, pull);

        // Check if the user is pulling anything, and drop it if so
        if (TryComp<PullerComponent>(ent, out var puller) && TryComp<PullableComponent>(puller.Pulling, out var pullable))
            _pullingSystem.TryStopPull(puller.Pulling.Value, pullable);

        var newCoords = _randomTeleport.GetRandomCoordinates(ent, implant.TeleportRadius);

        if (newCoords.HasValue)
        {
            _xform.SetCoordinates(ent, newCoords.Value);
            _audio.PlayPvs(implant.TeleportSound, ent);
            args.Handled = true;
        }
    }
    // RPSX - end
    // RPSX - RandomTeleport Refactor | Remove SelectRandomTileInRange

    private void OnDnaScramblerImplant(EntityUid uid, SubdermalImplantComponent component, UseDnaScramblerImplantEvent args)
    {
        if (component.ImplantedEntity is not { } ent)
            return;

        if (TryComp<HumanoidAppearanceComponent>(ent, out var humanoid))
        {
            var newProfile = HumanoidCharacterProfile.RandomWithSpecies(humanoid.Species);
            _humanoidAppearance.LoadProfile(ent, newProfile, humanoid);
            _metaData.SetEntityName(ent, newProfile.Name, raiseEvents: false); // raising events would update ID card, station record, etc.
            if (TryComp<DnaComponent>(ent, out var dna))
            {
                dna.DNA = _forensicsSystem.GenerateDNA();

                var ev = new GenerateDnaEvent { Owner = ent, DNA = dna.DNA };
                RaiseLocalEvent(ent, ref ev);
            }
            if (TryComp<FingerprintComponent>(ent, out var fingerprint))
            {
                fingerprint.Fingerprint = _forensicsSystem.GenerateFingerprint();
            }
            RemComp<DetailExaminableComponent>(ent); // remove MRP+ custom description if one exists 
            _identity.QueueIdentityUpdate(ent); // manually queue identity update since we don't raise the event
            _popup.PopupEntity(Loc.GetString("scramble-implant-activated-popup"), ent, ent);
        }

        args.Handled = true;
        QueueDel(uid);
    }
}
