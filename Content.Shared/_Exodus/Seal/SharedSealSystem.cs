using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Seal;

public abstract class SharedSealSystem : EntitySystem
{
    [Dependency] private readonly SharedIdCardSystem _idCardSystem = default!;
    [Dependency] protected readonly AccessReaderSystem AccessReader = default!;
    [Dependency] protected readonly SharedAudioSystem AudioSystem = default!;
    [Dependency] protected readonly SharedAppearanceSystem AppearanceSystem = default!;
    [Dependency] protected readonly SharedPopupSystem PopupSystem = default!;
    [Dependency] protected readonly SharedDoAfterSystem DoAfter = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<SealComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<SealComponent, ItemSlotEjectAttemptEvent>(OnEjectAttempt);
        SubscribeLocalEvent<SealComponent, ItemSlotInsertAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<SealComponent, GotEmaggedEvent>(OnEmag);
    }

    private void OnEjectAttempt(EntityUid uid, SealComponent component, ref ItemSlotEjectAttemptEvent args)
    {
        if (!component.Sealed)
            return;

        args.Cancelled = true;
    }

    private void OnInsertAttempt(EntityUid uid, SealComponent component, ref ItemSlotInsertAttemptEvent args)
    {
        if (!component.Sealed)
            return;

        args.Cancelled = true;
    }

    private void OnExamined(EntityUid uid, SealComponent component, ExaminedEvent args)
    {
        if (component.Sealed)
            args.PushText(Loc.GetString("seal-component-on-examine-is-sealed", ("entityName", Identity.Name(uid, EntityManager))));
    }

    protected bool CheckAccess(EntityUid uid, EntityUid user, AccessReaderComponent? reader = null, bool quiet = false)
    {
        if (!Resolve(uid, ref reader, false))
            return true;

        if (AccessReader.IsAllowed(user, uid, reader))
            return true;

        if (!quiet)
            PopupSystem.PopupEntity(Loc.GetString("seal-comp-access-fail"), uid, user);
        return false;
    }

    protected bool CheckIdentity(EntityUid uid, EntityUid user, SealComponent component, bool quiet = false)
    {
        if (component.NameToAccess == null)
            return true;

        if (!_idCardSystem.TryFindIdCard(user, out var idCard))
        {
            PopupSystem.PopupEntity(Loc.GetString("seal-comp-user-fail"), uid, user);
            return false;
        }
        if (!TryComp<IdCardComponent>(idCard, out var idcomp))
        {
            PopupSystem.PopupEntity(Loc.GetString("seal-comp-user-fail"), uid, user);
            return false;
        }
        if (idcomp.FullName == component.NameToAccess)
            return true;

        if (!quiet)
            PopupSystem.PopupEntity(Loc.GetString("seal-comp-user-fail"), uid, user);
        return false;
    }

    public void OnEmag(EntityUid uid, SealComponent component, ref GotEmaggedEvent args)
    {
        if (!component.BreakOnEmag && !component.Sealed)
            return;

        component.SealType = SealLockType.None;

        args.Handled = true;
    }

}

[Serializable, NetSerializable]
public enum SealLockType : byte
{
    /// <summary>
    /// No access needed
    /// </summary>
    None = 0,

    /// <summary>
    /// Access needed
    /// </summary>
    AccessSeal = 1,

    /// <summary>
    /// ID card of player needed
    /// </summary>
    IDNameSeal = 2,

    /// <summary>
    /// ID card of player and access needed
    /// </summary>
    AccessNameSeal = 3
}

