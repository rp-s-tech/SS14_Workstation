using Content.Client.UserInterface.Fragments;
using Content.Shared.RPSX.Bank.PDA;
using Robust.Client.UserInterface;

namespace Content.Client.RPSX.Bank.PDA.UI;

public sealed partial class PdaBankUi : UIFragment
{
    private PdaBankUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new PdaBankUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not BankCartridgeUiState bankState)
            return;

        _fragment?.UpdateState(bankState);
    }
}
