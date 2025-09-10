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
            // string format with 1 decimal place
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0.0}%", health.GetHealthPercentage().ToString());
        }
    }
}
