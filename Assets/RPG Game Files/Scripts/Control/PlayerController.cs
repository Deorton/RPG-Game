using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        MovementControl movementControl;
        Health health;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
            health = GetComponent<Health>();
        }

        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead()) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            
            print("Nothing to do");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                Health target = hit.transform.GetComponent<Health>();
                if (target == null) continue;

                if (!GetComponent<CombatControl>().CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<CombatControl>().Attack(target.gameObject);
                }

                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<MovementControl>().StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
