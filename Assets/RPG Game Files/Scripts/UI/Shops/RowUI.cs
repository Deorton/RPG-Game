using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image iconField = null;
        [SerializeField] TextMeshProUGUI nameField = null;
        [SerializeField] TextMeshProUGUI availabilityField = null;
        [SerializeField] TextMeshProUGUI itemPrice = null;
        [SerializeField] TextMeshProUGUI quantityField = null;

        public void Setup(ShopItem item)
        {
            iconField.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}";
            quantityField.text = "0"; // Placeholder until quantity in transaction is implemented
            itemPrice.text = $"${item.GetPrice():N2}";
        }
    }
}
