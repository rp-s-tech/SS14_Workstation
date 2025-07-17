using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Utility;
using Robust.Shared.Prototypes;
using Content.RPSX.Shared.GameRules.Pirates.Economics;

namespace Content.RPSX.Client.GameRules.Pirates.Economics.Shop
{
    public sealed class PirateShopBoundUserInterface : BoundUserInterface
    {
        private readonly PirateEconomicsSystem _shopSystem;

        [ViewVariables]
        private PirateShopWindow? _menu;

        /// <summary>
        /// This is the separate popup window for individual orders.
        /// </summary>
        [ViewVariables]
        private PirateShopOrderMenu? _orderMenu;

        [ViewVariables]
        public int Balance { get; private set; }

        /// <summary>
        /// Currently selected product
        /// </summary>
        [ViewVariables]
        private PirateStuffPrototype? _product;

        public PirateShopBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
            _shopSystem = EntMan.System<PirateEconomicsSystem>();
        }

        protected override void Open()
        {
            base.Open();

            var spriteSystem = EntMan.System<SpriteSystem>();
            var dependencies = IoCManager.Instance!;
            _menu = new PirateShopWindow(Owner, EntMan, dependencies.Resolve<IPrototypeManager>(), spriteSystem);
            var localPlayer = dependencies.Resolve<IPlayerManager>().LocalEntity;
            var description = new FormattedMessage();

            string orderRequester;

            if (EntMan.EntityExists(localPlayer))
                orderRequester = Identity.Name(localPlayer.Value, EntMan);
            else
                orderRequester = string.Empty;

            _orderMenu = new PirateShopOrderMenu();

            _menu.OnClose += Close;

            _menu.OnItemSelected += (args) =>
            {
                if (args.Button.Parent is not PirateProductRow row)
                    return;

                description.Clear();
                description.PushColor(Color.White); // Rich text default color is grey
                if (row.MainButton.ToolTip != null)
                    description.AddText(row.MainButton.ToolTip);

                _orderMenu.Description.SetMessage(description);
                _product = row.Product;
                _orderMenu.ProductName.Text = row.ProductName.Text;
                _orderMenu.PointCost.Text = row.PointCost.Text;
                _orderMenu.Amount.Value = 1;

                _orderMenu.OpenCentered();
            };

            _orderMenu.SubmitButton.OnPressed += (_) =>
            {
                if (AddOrder())
                {
                    _orderMenu.Close();
                }
            };

            _menu.OpenCentered();
        }

        private void Populate()
        {
            if (_menu == null)
                return;

            _menu.PopulateProducts();
            _menu.PopulateCategories();
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (state is not PirateShopInterfaceState cState)
                return;

            Balance = cState.Balance;

            if (_menu == null)
                return;

            _menu.Balance = Balance;
            _menu.ProductCatalogue = cState.Products;

            Populate();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            _menu?.Close();
            _orderMenu?.Close();

            _menu = null;
            _orderMenu = null;
        }

        private bool AddOrder()
        {
            var orderAmt = _orderMenu?.Amount.Value ?? 0;
            if (orderAmt <= 0 || orderAmt > Balance) return false;

            SendMessage(new PirateShopOrderMessage(
                _product?.Product ?? "",
                orderAmt,
                _product?.Cost ?? 0));

            return true;
        }
    }
}
