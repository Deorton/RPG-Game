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
        [SerializeField] TextMeshProUGUI totalField = null;

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
            if (currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }

            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            if (currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            currentShop.onChange += RefreshUI;

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
                RowUI row = Instantiate<RowUI>(rowPrefab, listRoot);
                row.Setup(currentShop, item);
            }

            totalField.text = $"Total: ${currentShop.TransactionTotal():N2}";
        }

        //Public Functions
        public void Close()
        {
            shopper.SetActiveShop(null);
        }
        
        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }
    }
}
