using System;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        CombatControl combatControl;

        void Awake()
        {
            combatControl = GameObject.FindWithTag("Player").GetComponent<CombatControl>();
        }

        void Update()
        {
            if (combatControl.GetTarget() == null)
            {
                GetComponent<TextMeshProUGUI>().text = "N/A";
                return;
            }

            Health health = combatControl.GetTarget().GetComponent<Health>();
            // string format with 1 decimal place
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0}%", health.GetHealthPercentage().ToString());
        }
    }
}
