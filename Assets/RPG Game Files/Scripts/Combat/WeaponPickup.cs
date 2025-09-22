using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<CombatControl>().EquipWeapon(weapon);
            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
