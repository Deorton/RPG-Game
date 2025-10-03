using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class FilterButtonUI : MonoBehaviour
    {
        Button button;
        Shop currentShop;
        [SerializeField] ItemCategory category = ItemCategory.None;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }
        
        public void SetSHop(Shop shop)
        {
            currentShop = shop;
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != category;
        }

        private void SelectFilter()
        {
            currentShop.SelectFilter(category);
        }
    }
}
