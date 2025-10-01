using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceField = null;
        Purse playerPurse = null;

        void Awake()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
        }

        void Start()
        {
            playerPurse.onChange += RefreshUI;
            RefreshUI();
        }

        void RefreshUI()
        {
            balanceField.text = $"${playerPurse.GetBalance():N2}";
        }
    }
}
