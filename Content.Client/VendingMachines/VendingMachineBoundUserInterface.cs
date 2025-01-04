using Content.Client.UserInterface.Controls;
using Content.Client.VendingMachines.UI;
using Content.Shared.VendingMachines;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using System.Linq;
using Content.Shared.RPSX.CCVars;
using Robust.Shared.Configuration;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.VendingMachines
{
    public sealed class VendingMachineBoundUserInterface : BoundUserInterface
    {
        [Dependency] private readonly IConfigurationManager _cfg = default!;

        [ViewVariables]
        private FancyWindow? _menu;

        [ViewVariables]
        private List<VendingMachineInventoryEntry> _cachedInventory = new();

        [ViewVariables]
        private List<int> _cachedFilteredIndex = new();

        public VendingMachineBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            if (!_cfg.GetCVar(RPSXCCVars.EconomyEnabled))
            {
                _menu = this.CreateWindow<VendingMachineMenu>();
                _menu.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;

                SetupOldVendingMenu((VendingMachineMenu)_menu);
            }
            else
            {
                _menu = this.CreateWindow<EconomyVendingMachineMenu>();
                _menu.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;

                SetupNewVendingMenu((EconomyVendingMachineMenu)_menu);
            }

            _menu.OnClose += Close;
            _menu.OpenCenteredLeft();
        }

        public void Refresh()
        {
            var system = EntMan.System<VendingMachineSystem>();
            _cachedInventory = system.GetAllInventory(Owner);
            if (_menu == null) return;

            if (_cfg.GetCVar(RPSXCCVars.EconomyEnabled))
            {
                var menu = (EconomyVendingMachineMenu)_menu;
                menu.Populate(_cachedInventory, out _cachedFilteredIndex);
            }
            else
            {
                var menu = (VendingMachineMenu)_menu;
                menu.Populate(_cachedInventory);
            }
        }


        private void SetupOldVendingMenu(VendingMachineMenu menu)
        {
            var system = EntMan.System<VendingMachineSystem>();
            _cachedInventory = system.GetAllInventory(Owner);
            menu.OnItemSelected += OnItemSelected;
            menu.Populate(_cachedInventory);
        }

        private void SetupNewVendingMenu(EconomyVendingMachineMenu menu)
        {
            var system = EntMan.System<VendingMachineSystem>();
            _cachedInventory = system.GetAllInventory(Owner);
            menu.OnItemSelected += OnItemSelected;
            menu.OnSearchChanged += OnSearchChanged;
            menu.OnBuyButtonPressed += OnBuyButtonPressed;
            menu.OnSelectedItemRequestUpdate += OnSelectedItemRequestUpdate;
            menu.Populate(_cachedInventory, out _cachedFilteredIndex);
        }

        private void OnSelectedItemRequestUpdate(int index)
        {
            var selected = GetSelectedItem(index);
            if (selected != null && _menu is EconomyVendingMachineMenu economyMenu)
            {
                economyMenu.SetSelectedProductState(selected, index);
            }
        }

        private void OnItemSelected(GUIBoundKeyEventArgs args, ListData data)
        {
            if (args.Function != EngineKeyFunctions.UIClick)
                return;

            if (data is not VendorItemsListData { ItemIndex: var itemIndex })
                return;

            if (_cachedInventory.Count == 0)
                return;

            var selectedItem = _cachedInventory.ElementAtOrDefault(itemIndex);

            if (selectedItem == null)
                return;

            SendMessage(new VendingMachineEjectMessage(selectedItem.Type, selectedItem.ID));
        }
        /*
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            if (_menu == null)
                return;

            _menu.OnItemSelected -= OnItemSelected;
            _menu.OnClose -= Close;
            _menu.Dispose();
        }

        */
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            var system = EntMan.System<VendingMachineSystem>();
            _cachedInventory = system.GetAllInventory(Owner);

            switch (_menu)
            {
                case EconomyVendingMachineMenu economyMenu:
                    economyMenu.Populate(_cachedInventory, out _cachedFilteredIndex, economyMenu.SearchBar.Text);
                    economyMenu.UpdateSelectedProduct();
                    break;
                case VendingMachineMenu menu:
                    menu.Populate(_cachedInventory);
                    break;
            }
        }

        private void OnItemSelected(ItemList.ItemListSelectedEventArgs args)
        {
            var selectedItem = GetSelectedItem(args.ItemIndex);
            if (selectedItem == null)
                return;

            switch (_menu)
            {
                case EconomyVendingMachineMenu economyMenu:
                    economyMenu.SetSelectedProductState(selectedItem, args.ItemIndex);
                    break;
                case VendingMachineMenu:
                    SendMessage(new VendingMachineEjectMessage(selectedItem.Type, selectedItem.ID));
                    break;
            }
        }

        private void OnBuyButtonPressed(int index)
        {
            var selectedItem = GetSelectedItem(index);
            if (selectedItem == null)
                return;

            SendMessage(new VendingMachineEjectMessage(selectedItem.Type, selectedItem.ID));
            Refresh();
        }

        private VendingMachineInventoryEntry? GetSelectedItem(int index)
        {
            return _cachedInventory.Count == 0
                ? null
                : _cachedInventory.ElementAtOrDefault(_cachedFilteredIndex.ElementAtOrDefault(index));
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            switch (_menu)
            {
                case null:
                    return;
                case EconomyVendingMachineMenu economyMenu:
                    economyMenu.OnItemSelected -= OnItemSelected;
                    break;
                case VendingMachineMenu menu:
                    menu.OnItemSelected -= OnItemSelected;
                    break;
            }

            _menu.OnClose -= Close;
            _menu.Dispose();
        }

        private void OnSearchChanged(string? filter)
        {
            switch (_menu)
            {
                case null:
                    return;
                case EconomyVendingMachineMenu economyMenu:
                    economyMenu.Populate(_cachedInventory, out _cachedFilteredIndex, filter);
                    break;
                case VendingMachineMenu menu:
                    menu.Populate(_cachedInventory);
                    break;
            }
        }
    }
}
