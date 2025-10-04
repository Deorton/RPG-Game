using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Demo Targetting", menuName = "RPG Game/Abilities/Targeting/Demo", order = 0)]
    public class Demotargetting : TargetingStrategy
    {
        public override void StartTargetting(GameObject user)
        {
            Debug.Log("Demo Targetting Started");
        }
    }
}
