using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Exodus.Seal;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.Seal;

public sealed class SealSystem : SharedSealSystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SealComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<SealComponent, ActivateInWorldEvent>(OnActivated);
        SubscribeLocalEvent<SealComponent, UnsealDoAfter>(OnDoAfter);
        SubscribeLocalEvent<SealComponent, GetVerbsEvent<AlternativeVerb>>(AddUnsealVerb);
    }

    private void OnStartup(EntityUid uid, SealComponent component, ComponentStartup args)
    {
        AppearanceSystem.SetData(uid, SealVisual.Sealed, true);
    }
    public void OnActivated(EntityUid uid, SealComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        if (component.Sealed)
        {
            TryUnseal(uid, args.User, component);
            args.Handled = true;
        }
    }

    public bool CanStartUnseal(EntityUid uid, EntityUid user, bool quiet = true)
    {
        if (!HasComp<HandsComponent>(user))
            return false;

        var ev = new UnsealAttemptEvent(user, quiet);
        RaiseLocalEvent(uid, ref ev, true);
        return !ev.Cancelled;
    }

    public bool CanUnsealByAccess(EntityUid uid, EntityUid user, SealComponent component, bool quiet = true)
    {
        switch (component.SealType)
        {
            case SealLockType.None:
                return true;

            case SealLockType.AccessSeal:
                return CheckAccess(uid, user, quiet: false);

            case SealLockType.IDNameSeal:
                return CheckIdentity(uid, user, component, quiet: false);

            case SealLockType.AccessNameSeal:
                return (CheckAccess(uid, user, quiet: false) && CheckIdentity(uid, user, component, quiet: false));

            default:
                return false;
        }
    }

    public bool TryUnseal(EntityUid uid, EntityUid user, SealComponent? component = null, bool skipDoAfter = false)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (!CanStartUnseal(uid, user, quiet: false))
            return false;

        if (!CanUnsealByAccess(uid, user, component, quiet: false))
            return false;

        if (!skipDoAfter && component.UnsealTime != TimeSpan.Zero)
        {
            return DoAfter.TryStartDoAfter(new DoAfterArgs(EntityManager, user, component.UnsealTime, new UnsealDoAfter(), uid, uid)
            {
                BreakOnDamage = true, BreakOnMove = true, RequireCanInteract = true,
                NeedHand = true
            });
        }

        Unseal(uid, user, component);
        return true;

    }

    public void OnDoAfter(EntityUid uid, SealComponent component, UnsealDoAfter args)
    {
        if (args.Cancelled)
            return;

        TryUnseal(uid, args.User, component, true);
    }

    public void Unseal(EntityUid uid, EntityUid? user, SealComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (user is not null)
        {
            PopupSystem.PopupEntity(Loc.GetString("seal-comp-do-unseal-success", ("entityName", Identity.Name(uid, EntityManager))), uid, user.Value);
            AudioSystem.PlayPredicted(component.UnsealingSound, uid, user.Value);
        }

        component.Sealed = false;
        AppearanceSystem.SetData(uid, SealVisual.Sealed, false);
        Dirty(uid, component);

        if (component.WillAnnounce)
            MakeAnnouncement(uid, component);

        var ev = new UnsealedEvent();
        RaiseLocalEvent(uid, ref ev, true);

        if (component.SpawnOnUnseal != null)
            SpawnNextToOrDrop(component.SpawnOnUnseal, uid);

        if (component.RemoveOnUnseal)
            RemComp(uid, component);
    }

    private void AddUnsealVerb(EntityUid uid, SealComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!component.Sealed)
            return;

        AlternativeVerb verb = new()
        {
            Act = () => TryUnseal(uid, args.User, component),
            Text = Loc.GetString("unseal-verb"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png")),
        };
        args.Verbs.Add(verb);
    }

    public void MakeAnnouncement(EntityUid uid, SealComponent component)
    {
        if (component.AnnounceText.HasValue && component.AnnounceTitle.HasValue)
            _chatSystem.DispatchStationAnnouncement(uid, Loc.GetString(component.AnnounceText), Loc.GetString(component.AnnounceTitle), colorOverride: Color.Red);
    }
}
