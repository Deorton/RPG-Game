using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] MovementControl movementControl;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
        }

        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            InteractWithCombat();
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }
        }

        private void InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                Health target = hit.transform.GetComponent<Health>();
                if (target == null) continue;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<CombatControl>().Attack(target);
                }
            }
        }

        private void MoveToCursor()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                movementControl.MoveTo(hit.point);
            }
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
