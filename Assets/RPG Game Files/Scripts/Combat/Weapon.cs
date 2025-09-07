using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Game/Weapons/New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
         [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damage = 10f;
        [SerializeField] bool isRightHanded = true;

        public float GetRange()
        {
            return attackRange;
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (equippedPrefab != null)
            {
                Transform handTransform = isRightHanded ? rightHand : leftHand;
                Instantiate(equippedPrefab, handTransform);
            }

            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
        }
    }
}
