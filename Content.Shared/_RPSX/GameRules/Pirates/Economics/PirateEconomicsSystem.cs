using Content.Shared.Access.Systems;
using Content.Shared.Popups;
using Content.Shared.RPSX.GameRules.Pirates;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Shared.RPSX.GameRules.Pirates.Economics;

public sealed class PirateEconomicsSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AccessReaderSystem _accessReaderSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    private HashSet<EntityUid> _setEnts = new();
    private List<(EntityUid, PiratePalletComponent, TransformComponent)> _pads = new();
    private static readonly SoundSpecifier ApproveSound = new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");
    private ProtoId<TagPrototype> _sellTag = "PiratesSellable";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PirateShopComponent, BoundUIOpenedEvent>(OnOpenedUI);
        SubscribeLocalEvent<PirateShopComponent, PirateShopOrderMessage>(OnBuyedProduct);
        SubscribeLocalEvent<PirateShopComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<PiratePalletConsoleComponent, PiratePalletSellMessage>(OnPalletSale);
        SubscribeLocalEvent<PiratePalletConsoleComponent, PiratePalletAppraiseMessage>(OnPalletAppraise);
        SubscribeLocalEvent<PiratePalletConsoleComponent, BoundUIOpenedEvent>(OnPalletUIOpen);
    }

    #region Helpers
    private void ConsolePopup(EntityUid actor, string text)
    {
        _popup.PopupPredictedCursor(text, actor);
    }

    private void PlayDenySound(Entity<PirateShopComponent> entity, EntityUid? user)
    {
        if (_timing.CurTime >= entity.Comp.NextDenySoundTime)
        {
            entity.Comp.NextDenySoundTime = _timing.CurTime + entity.Comp.DenySoundDelay;
            _audio.PlayPredicted(entity.Comp.ErrorSound, entity, user);
        }
    }

    public void ChangePiratesBalance(Entity<PiratesProgressComponent> progress, int summ)
    {
        progress.Comp.Balance += summ;
        Dirty(progress);
    }

    private Entity<PiratesProgressComponent>? GetProgress(EntityUid entity)
    {
        var query = EntityQueryEnumerator<PiratesProgressComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.PiratesOutpost is not { } outpost) continue;
            if (Transform(outpost).GridUid == Transform(entity).GridUid)
                return (uid, comp);
        }
        return null;
    }

    #endregion

    #region Shopping
    private void OnMapInit(Entity<PirateShopComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.NextTick = _timing.CurTime + entity.Comp.NextTickDelay;
    }

    private void OnOpenedUI(Entity<PirateShopComponent> entity, ref BoundUIOpenedEvent args)
    {
        UpdateState(entity);
    }

    private void OnBuyedProduct(Entity<PirateShopComponent> entity, ref PirateShopOrderMessage args)
    {
        if (GetProgress(entity) is not { } progress) return;
        if (args.Actor is not { Valid: true } player)
        {
            PlayDenySound(entity, null);
            return;
        }

        if (!_accessReaderSystem.IsAllowed(player, entity))
        {
            ConsolePopup(args.Actor, Loc.GetString("cargo-console-order-not-allowed"));
            PlayDenySound(entity, player);
            return;
        }

        var summ = -1 * args.Cost;
        ChangePiratesBalance(progress, summ);
        entity.Comp.ProductsQueue.Add((args.ProductId, args.Amount));
        UpdateState(entity);
        Dirty(entity);
    }

    private void UpdateState(Entity<PirateShopComponent> entity)
    {
        if (!_ui.HasUi(entity, PirateShopUiKey.Key)) return;
        if (GetProgress(entity) is not { } progress) return;
        var products = new List<ProtoId<PirateStuffPrototype>>();
        var productsPrototypes = _prototypeManager.EnumeratePrototypes<PirateStuffPrototype>().ToHashSet();
        foreach (var product in productsPrototypes)
        {
            products.Add(product.ID);
        }
        var state = new PirateShopInterfaceState(products, progress.Comp.Balance);
        _ui.SetUiState(entity.Owner, PirateShopUiKey.Key, state);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PirateShopComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var pirateShop, out var xform))
        {

            if (_timing.CurTime < pirateShop.NextTick)
                continue;

            pirateShop.NextTick += pirateShop.NextTickDelay;

            if (pirateShop.ProductsQueue.Count == 0)
                continue;

            var (protoid, count) = pirateShop.ProductsQueue[0];

            SpawnAtPosition(protoid, xform.Coordinates);
            pirateShop.ProductsQueue[0] = (protoid, --count);

            if (pirateShop.ProductsQueue[0].Item2 <= 0)
                pirateShop.ProductsQueue.RemoveAt(0);

            Dirty(uid, pirateShop);
        }
    }
    #endregion

    #region Selling
    private void UpdatePalletConsoleInterface(EntityUid uid)
    {
        if (Transform(uid).GridUid is not { } gridUid)
        {
            _ui.SetUiState(uid,
                PiratePalletConsoleUiKey.Key,
                new PiratePalletConsoleInterfaceState(0, 0, false));
            return;
        }
        GetPalletGoods(gridUid, out var toSell, out var goods);
        var totalAmount = goods.Sum(t => t.Item2);
        _ui.SetUiState(uid,
            PiratePalletConsoleUiKey.Key,
            new PiratePalletConsoleInterfaceState((int) totalAmount, toSell.Count, true));
    }

    private void OnPalletUIOpen(EntityUid uid, PiratePalletConsoleComponent component, BoundUIOpenedEvent args)
    {
        UpdatePalletConsoleInterface(uid);
    }

    private void OnPalletAppraise(EntityUid uid, PiratePalletConsoleComponent component, PiratePalletAppraiseMessage args)
    {
        UpdatePalletConsoleInterface(uid);
    }

    private bool SellPallets(EntityUid gridUid, out HashSet<(EntityUid, double)> goods)
    {
        GetPalletGoods(gridUid, out var toSell, out goods);

        if (toSell.Count == 0)
            return false;

        foreach (var ent in toSell)
        {
            Del(ent);
        }

        return true;
    }

    private void GetPalletGoods(EntityUid gridUid, out HashSet<EntityUid> toSell, out HashSet<(EntityUid, double)> goods)
    {
        goods = new HashSet<(EntityUid, double)>();
        toSell = new HashSet<EntityUid>();

        foreach (var (palletUid, _, _) in GetCargoPallets(gridUid))
        {
            _setEnts.Clear();

            _lookup.GetEntitiesIntersecting(
                palletUid,
                _setEnts,
                LookupFlags.Dynamic | LookupFlags.Sundries);

            foreach (var ent in _setEnts)
            {
                if (toSell.Contains(ent) ||
                    Transform(ent).Anchored ||
                    _tag.HasTag(ent, _sellTag))
                {
                    continue;
                }

                var ev = new PiratePriceCalculationEvent();
                RaiseLocalEvent(ent, ref ev);
                if (ev.Handled)
                    continue;

                toSell.Add(ent);
                goods.Add((ent, Math.Round(ev.Price / 100)));
            }
        }
    }

    private void OnPalletSale(EntityUid uid, PiratePalletConsoleComponent component, PiratePalletSellMessage args)
    {
        if (GetProgress(uid) is not { } progress) return;

        if (Transform(uid).GridUid is not { } gridUid)
        {
            _ui.SetUiState(uid,
                PiratePalletConsoleUiKey.Key,
                new PiratePalletConsoleInterfaceState(0, 0, false));
            return;
        }

        if (!SellPallets(gridUid, out var goods))
            return;

        var sum = (int)Math.Round(goods.Sum(tuple => tuple.Item2));
        ChangePiratesBalance(progress, sum);

        _audio.PlayPredicted(ApproveSound, uid, null);
        UpdatePalletConsoleInterface(uid);
    }

    private List<(EntityUid Entity, PiratePalletComponent Component, TransformComponent PalletXform)> GetCargoPallets(EntityUid gridUid)
    {
        _pads.Clear();

        var query = AllEntityQuery<PiratePalletComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var comp, out var compXform))
        {
            if (compXform.ParentUid != gridUid ||
                !compXform.Anchored)
            {
                continue;
            }
            _pads.Add((uid, comp, compXform));
        }

        return _pads;
    }
    #endregion
}
