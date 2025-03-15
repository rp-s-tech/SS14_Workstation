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
using Robust.Shared.GameObjects;

namespace Content.Client.RPSX.Bank.PDA.UI.Cartridges;

public sealed partial class HeadShopUi : UIFragment
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private HeadShopUiFragment? _fragment;
    private CargoConsoleOrderMenu? _orderMenu;
    protected readonly SharedUserInterfaceSystem UiSystem;
    private EntityUid _owner;

    [ViewVariables]
    public int OrderCapacity { get; private set; }

    [ViewVariables]
    private CargoProductPrototype? _product;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        if (fragmentOwner == null) return;

        _owner = fragmentOwner.Value;
        string orderRequester;
        var description = new FormattedMessage();

        _orderMenu = new CargoConsoleOrderMenu();

        _fragment = new HeadShopUiFragment();

        if (userInterface != null && _entityManager.TryGetComponent<MetaDataComponent>(userInterface.Owner, out var metadata))
            orderRequester = Identity.Name(userInterface.Owner, _entityManager);
        else
            orderRequester = string.Empty;

        _fragment.OnItemSelected += (args) =>
        {
            if (args.Button.Parent is not CargoProductRow row)
                return;

            description.Clear();
            description.PushColor(Color.White); // Rich text default color is grey
            if (row.MainButton.ToolTip != null)
                description.AddText(row.MainButton.ToolTip);

            _orderMenu.Description.SetMessage(description);
            _product = row.Product;
            _orderMenu.ProductName.Text = row.ProductName.Text;
            _orderMenu.PointCost.Text = row.PointCost.Text;
            _orderMenu.Requester.Text = orderRequester;
            _orderMenu.Reason.Text = "";
            _orderMenu.Amount.Value = 1;

            _orderMenu.OpenCentered();
        };

        _orderMenu.SubmitButton.OnPressed += (_) =>
        {
            if (userInterface != null && AddOrder(userInterface))
            {
                _orderMenu.Close();
            }
        };
    }

    private bool AddOrder(BoundUserInterface userInterface)
    {
        var orderAmt = _orderMenu?.Amount.Value ?? 0;
        if (orderAmt < 1 || orderAmt > OrderCapacity)
        {
            return false;
        }

        userInterface.SendMessage(new CargoConsoleAddOrderMessage(
            _orderMenu?.Requester.Text ?? "",
            _orderMenu?.Reason.Text ?? "",
            _product?.ID ?? "",
            orderAmt));

        return true;
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not CargoConsoleInterfaceState bankState)
            return;

        _fragment?.UpdateState(bankState, _owner);
    }
}
