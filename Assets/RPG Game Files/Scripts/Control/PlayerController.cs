using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        MovementControl movementControl;
        Health health;

        enum CursorType
        {
            None,
            Movement,
            Combat,
            UI
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

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
            if (InteractWithUI()) return;

            if (health.IsDead())
            {
                SetCursor(CursorType.UI);
                return;
            }

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            // If over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
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
                SetCursor(CursorType.Combat);
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
                    GetComponent<MovementControl>().StartMoveAction(hit.point, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType combat)
        {
            CursorMapping mapping = GetCursorMapping(combat);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
    }
}
