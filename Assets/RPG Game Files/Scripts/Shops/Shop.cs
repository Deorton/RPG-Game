using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        [SerializeField] string shopName;
        [Range(0, 100)]
        [SerializeField] float sellingPercentage = 80f;

        [SerializeField] StockItemConfig[] stockConfig;

        [Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)]
            public float buyDiscountPercentage;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();

        Shopper currentShopper = null;

        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;    

        public event Action onChange;

        void Awake()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                stock[config.item] = config.initialStock;
            }
        }

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            return GetAllItems();
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                float price = GetPrice(config);
                int quantityInTransaction = 0;
                transaction.TryGetValue(config.item, out quantityInTransaction);
                int availability = GetAvailability(config.item);
                yield return new ShopItem(config.item, availability, price, quantityInTransaction);
            }
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;

            if (onChange != null)
            {
                onChange();
            }
        }

        public ItemCategory GetFilter()
        {
            return filter;
        }

        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;

            if (onChange != null)
            {
                onChange();
            }
        }

        public bool IsBuyingMode()
        {
            return isBuyingMode;
        }

        public bool CanTransact()
        {
            if (transactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        public bool HasInventorySpace()
        {
            if (!isBuyingMode) return true;

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }
            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true;

            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperPurse == null) return false;

            return shopperPurse.GetBalance() >= TransactionTotal();
        }

        public bool transactionEmpty()
        {
            return transaction.Count == 0;
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            // Transfer to or from the inventory
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();

                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }
            // Removal from transaction
            // Debting or Crediting of funds

            if (onChange != null)
            {
                onChange();
            }
        }

        public float TransactionTotal()
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }

            return total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            int availability = GetAvailability(item);
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            if (onChange != null)
            {
                onChange();
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }

        public string GetShopName()
        {
            return shopName;
        }

        private int GetAvailability(InventoryItem item)
        {
            if (isBuyingMode)
            {
                return stock[item];
            }

            return CountItemsInInventory(item);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            if (inventory == null) return 0;

            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        private float GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return config.item.GetPrice() * (1 - config.buyDiscountPercentage / 100f);
            }
            else
            {
                return config.item.GetPrice() * (sellingPercentage / 100f);
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            stock[item]++;
            shopperPurse.UpdateBalance(price);
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stock[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }
        
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
