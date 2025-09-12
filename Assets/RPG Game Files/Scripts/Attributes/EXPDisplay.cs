using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class EXPDisplay : MonoBehaviour
    {
        Experience experience;

        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            // string format with 1 decimal place
            GetComponent<TextMeshProUGUI>().text = experience.GetEXP().ToString();
        }
    }
}
