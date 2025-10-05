using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Demo Targetting", menuName = "RPG Game/Abilities/Targeting/Demo", order = 0)]
    public class Demotargetting : TargetingStrategy
    {
        public override void StartTargetting(GameObject user, Action<IEnumerable<GameObject>> finshed)
        {
            Debug.Log("Demo Targetting Started");
            finshed(null);
        }
    }
}
