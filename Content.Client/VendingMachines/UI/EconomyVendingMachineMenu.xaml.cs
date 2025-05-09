using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.VendingMachines;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Content.Shared.IdentityManagement;

namespace Content.Client.VendingMachines.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class EconomyVendingMachineMenu : FancyWindow
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        public event Action<ItemList.ItemListSelectedEventArgs>? OnItemSelected;

        public event Action<int>? OnSelectedItemRequestUpdate;
        public event Action<string>? OnSearchChanged;
        public event Action<int>? OnBuyButtonPressed;

        private int _selectedItemIndex = -1;

        public EconomyVendingMachineMenu()
        {
            MinSize = new Vector2(500, 500);
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            SearchBar.OnTextChanged += _ =>
            {
                OnSearchChanged?.Invoke(SearchBar.Text);
            };

            VendingContents.OnItemSelected += args =>
            {
                OnItemSelected?.Invoke(args);
            };
            BuyButton.OnPressed += _ =>
            {
                OnBuyButtonPressed?.Invoke(_selectedItemIndex);
            };

            SplitContainer.State = SplitContainer.SplitState.Auto;
            SplitContainer.ResizeMode = SplitContainer.SplitResizeMode.NotResizable;
            SplitContainer.SplitWidth = 2;
            SplitContainer.SplitEdgeSeparation = 1f;
            SplitContainer.StretchDirection = SplitContainer.SplitStretchDirection.TopLeft;
        }

        public void SetSelectedProductState(VendingMachineInventoryEntry selectedProduct, int index)
        {
            var spriteSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>();
            (var name, Texture? icon) = GetItemNameAndIcon(selectedProduct.ID, spriteSystem);

            ChosenProductIcon.Texture = icon;
            ChosenProduct.Text = name;
            ChosenProductAmount.Text = Loc.GetString("vending-machine-component-product-amount", ("amount", selectedProduct.Amount));
            ChosenProductPrice.Text = selectedProduct.Price > 0 ? Loc.GetString("vending-machine-component-price", ("price", selectedProduct.Price)) : Loc.GetString("vending-machine-component-price-free");

            ChosenProduct.Visible = true;
            ChosenProductIcon.Visible = true;

            ChosenProductCountContainer.Visible = selectedProduct.Amount > 0;
            ChosenProductPriceContainer.Visible = selectedProduct.Amount > 0;
            ChosenProductEnd.Visible = selectedProduct.Amount <= 0;

            ProductNotSelected.Visible = false;

            BuyButton.Text = selectedProduct.Price > 0 ? Loc.GetString("vending-machine-component-buy") : Loc.GetString("vending-machine-component-get");
            BuyButton.Visible = selectedProduct.Amount > 0;
            _selectedItemIndex = index;
        }

        /// <summary>
        /// Populates the list of available items on the vending machine interface
        /// and sets icons based on their prototypes
        /// </summary>
        public void Populate(List<VendingMachineInventoryEntry> inventory, out List<int> filteredInventory,
            string? filter = null)
        {
            filteredInventory = new List<int>();

            if (inventory.Count == 0)
            {
                VendingContents.Clear();
                var outOfStockText = Loc.GetString("vending-machine-component-try-eject-out-of-stock");
                VendingContents.AddItem(outOfStockText);
                return;
            }

            while (inventory.Count != VendingContents.Count)
            {
                if (inventory.Count > VendingContents.Count)
                    VendingContents.AddItem(string.Empty);
                else
                    VendingContents.RemoveAt(VendingContents.Count - 1);
            }

            var longestEntry = string.Empty;
            var spriteSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>();

            var filterCount = 0;
            for (var i = 0; i < inventory.Count; i++)
            {
                var entry = inventory[i];
                var vendingItem = VendingContents[i - filterCount];
                vendingItem.Text = string.Empty;
                vendingItem.Icon = null;

                (var item, Texture? icon) = GetItemNameAndIcon(entry.ID, spriteSystem);
                var dummy = _entityManager.Spawn(entry.ID);
                var itemName = Identity.Name(dummy, _entityManager);
                var itemText = $"{itemName} [{entry.Amount}]";

                // search filter
                if (!string.IsNullOrEmpty(filter) &&
                    !itemText.ToLowerInvariant().Contains(filter.Trim().ToLowerInvariant()))
                {
                    VendingContents.Remove(vendingItem);
                    filterCount++;
                    continue;
                }

                if (itemText.Length > longestEntry.Length)
                    longestEntry = itemText;

                vendingItem.Text = $"{itemText}";
                vendingItem.Icon = icon;
                filteredInventory.Add(i);
            }
            SetSizeAfterUpdate(longestEntry.Length);
        }

        private (string, Texture?) GetItemNameAndIcon(string id, SpriteSystem spriteSystem)
        {
            if (_prototypeManager.TryIndex<EntityPrototype>(id, out var prototype))
            {
                return (prototype.Name, spriteSystem.GetPrototypeIcon(prototype).Default);
            }

            return (id, null);
        }

        public void UpdateSelectedProduct()
        {
            if (_selectedItemIndex != -1)
            {
                OnSelectedItemRequestUpdate?.Invoke(_selectedItemIndex);
            }
        }
        private void SetSizeAfterUpdate(int longestEntryLength)
        {
            longestEntryLength = longestEntryLength * 12 - 50;
            if (longestEntryLength < 250)
                VendingContents.SetWidth = 250;
            else if (longestEntryLength > 400)
                VendingContents.SetWidth = 400;
            else
                VendingContents.SetWidth = longestEntryLength;
        }
    }
}
