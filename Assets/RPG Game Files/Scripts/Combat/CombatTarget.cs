using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<CombatControl>().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<CombatControl>().Attack(gameObject);
            }
                
            return true;
        }
    }
}
