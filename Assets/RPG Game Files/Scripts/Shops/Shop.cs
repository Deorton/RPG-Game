using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        [SerializeField] string shopName;

        public event Action onChange;

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            yield return new ShopItem(InventoryItem.GetFromID("a74a9c7b-c37f-44b5-b896-de071302b635"), 10, 500, 0);
            yield return new ShopItem(InventoryItem.GetFromID("38f1c5a6-19e0-4084-b136-86ced8ed18a1"), 10, 500, 0);
            yield return new ShopItem(InventoryItem.GetFromID("56d06e4b-64fd-4615-b44d-e06a54e42774"), 10, 500, 0);
        }

        public void SelectFilter(ItemCategory category) { }
        public ItemCategory GetFilter() { return ItemCategory.None; }
        public void SelectMode(bool isBuying) { }
        public bool IsBuyingMode() { return true; }
        public bool CanTransact() { return true; }
        public void ConfirmTransaction() { }
        public float TransactionTotal() { return 0; }
        public void AddToTransaction(InventoryItem item, int quantity) { }

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
    }
}
