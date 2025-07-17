using Content.RPSX.Shared.GameRules.Pirates.Economics;
using Content.Shared.Cargo.BUI;
using Robust.Client.UserInterface;

namespace Content.RPSX.Client.GameRules.Pirates.Economics.Pallets;

public sealed class PiratePalletConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private PiratePalletMenu? _menu;

    public PiratePalletConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<PiratePalletMenu>();
        _menu.AppraiseRequested += OnAppraisal;
        _menu.SellRequested += OnSell;
    }

    private void OnAppraisal()
    {
        SendMessage(new PiratePalletAppraiseMessage());
    }

    private void OnSell()
    {
        SendMessage(new PiratePalletSellMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not PiratePalletConsoleInterfaceState palletState)
            return;

        _menu?.SetEnabled(palletState.Enabled);
        _menu?.SetAppraisal(palletState.Appraisal);
        _menu?.SetCount(palletState.Count);
    }
}
