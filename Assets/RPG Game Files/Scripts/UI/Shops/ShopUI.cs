using RPG.Shops;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName = null;
        [SerializeField] Transform listRoot = null;
        [SerializeField] RowUI rowPrefab = null;
        [SerializeField] TextMeshProUGUI totalField = null;
        [SerializeField] Button confirmButton = null;
        [SerializeField] Button switchModeButton = null;

        Shopper shopper = null;
        Shop currentShop = null;

        Color originalTotalColor;

        void Start()
        {
            originalTotalColor = totalField.color;

            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;

            shopper.activeShopChanged += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchModeButton.onClick.AddListener(SwitchMode);

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

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetSHop(currentShop);
            }

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
            totalField.color = currentShop.HasSufficientFunds() ? originalTotalColor : Color.red;
            confirmButton.interactable = currentShop.CanTransact();

            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI switchText = switchModeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode())
            {
                switchText.text = "Switch to Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
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
        
        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}
