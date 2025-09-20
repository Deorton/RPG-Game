using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            // string format with 1 decimal place to show percentage
        //    GetComponent<TextMeshProUGUI>().text = String.Format("{0:0.0}%", health.GetHealthPercentage().ToString());

            // alternative format to show current and max health points
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0} / {1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
