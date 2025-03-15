using Content.Client.Cargo.UI;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Containers;
using Robust.Shared.Log;
using Content.Shared.RPSX.Bank.PDA;

namespace Content.Client.RPSX.Bank.PDA.UI.Cartridges;

public sealed partial class HeadShopUi : UIFragment
{
    private HeadShopUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        SendBUI(userInterface);
        _fragment = new HeadShopUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not HeadShopCartridgeInterfaceState bankState)
            return;

        _fragment?.UpdateState(bankState);
    }

    private void SendBUI(BoundUserInterface userInterface)
    {
        _fragment?.GetBUI(userInterface);
    }
}
