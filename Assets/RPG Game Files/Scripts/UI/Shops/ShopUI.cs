using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shops;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName = null;
        [SerializeField] Transform listRoot = null;
        [SerializeField] RowUI rowPrefab = null;
        Shopper shopper = null;
        Shop currentShop = null;

        void Start()
        {
            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;

            shopper.activeShopChanged += ShopChanged;

            ShopChanged();
        }

        void ShopChanged()
        {
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            if (currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform item in listRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (ShopItem item in currentShop.GetFilteredItems())
            {
                Instantiate<RowUI>(rowPrefab, listRoot);
            //    row.Setup(item, currentShop);
            }
        }

        //Public Functions
        public void Close()
        {
            shopper.SetActiveShop(null);
        }
    }
}
