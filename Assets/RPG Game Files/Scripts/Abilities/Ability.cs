using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "RPG Game/Abilities/New Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargetting(user, TargetAquired);
        }

        private void TargetAquired(IEnumerable<GameObject> targets)
        {
            Debug.Log("Target Aquired");

            foreach (var target in targets)
            {
                Debug.Log(target);
            }
        }
    }
}
