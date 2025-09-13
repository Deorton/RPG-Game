using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class LevelDisplay : MonoBehaviour
    {
         BaseStats baseStats;

        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            // string format with 1 decimal place
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}
