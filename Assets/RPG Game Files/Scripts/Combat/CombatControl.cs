using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class CombatControl : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Attack(Health target)
        {
            Debug.Log(name + " Attacking " + target.name);
        }
    }
}

