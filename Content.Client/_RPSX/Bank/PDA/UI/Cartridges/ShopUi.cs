using Content.Client.UserInterface.Fragments;
using Robust.Client.UserInterface;
using Content.Shared.RPSX.Bank.PDA;

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
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not ShopCartridgeInterfaceState bankState)
            return;

        _fragment?.UpdateState(bankState);
    }
}
