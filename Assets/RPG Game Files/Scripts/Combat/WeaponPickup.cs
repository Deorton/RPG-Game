using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                PickUp(other.GetComponent<CombatControl>());
            }
        }

        private void PickUp(CombatControl combatControl)
        {
            combatControl.EquipWeapon(weapon);
            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.GetComponent<CombatControl>());
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
