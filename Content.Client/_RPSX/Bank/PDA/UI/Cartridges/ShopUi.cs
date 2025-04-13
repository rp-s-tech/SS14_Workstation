using Content.Client.UserInterface.Fragments;
using Robust.Client.UserInterface;
using Content.Shared.RPSX.Bank.PDA;
using Content.Shared.RPSX.Bank.Prototypes;
using Content.Shared.StationRecords;
using Content.Shared.CartridgeLoader;

namespace Content.Client.RPSX.Bank.PDA.UI.Cartridges;

public sealed partial class ShopUi : UIFragment
{
    private ShopUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new ShopUiFragment();
        _fragment.OnBasketUpdated += args => SendMessage(new ShopUpdateMessage(args.Item1, args.Item2, args.Item3), userInterface);
        _fragment.OnBasketBuyed += args => SendMessage(new ShopBuyMessage(args.Item1, args.Item2), userInterface);
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not ShopCartridgeInterfaceState bankState)
            return;

        _fragment?.UpdateState(bankState);
    }

    private void SendMessage(CartridgeMessageEvent message, BoundUserInterface userInterface)
    {
        userInterface.SendMessage(new CartridgeUiMessage(message));
    }
}
