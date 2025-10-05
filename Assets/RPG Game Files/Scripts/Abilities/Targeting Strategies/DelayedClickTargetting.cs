using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "RPG Game/Abilities/Targeting/Delayed Click", order = 0)]
    public class DelayedClickTargetting : TargetingStrategy
    {
        PlayerController playerController;
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] Vector2 cursorHotspot;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaAffectRadius;
        [SerializeField] Transform targetingPrefab;

        Transform targetingPrefabInstance = null;

        public override void StartTargetting(GameObject user, Action<IEnumerable<GameObject>> finshed)
        {
            PlayerController playerController = user.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(user, playerController, finshed));
        }

        private IEnumerator Targeting(GameObject user, PlayerController playerController, Action<IEnumerable<GameObject>> finshed)
        {
            playerController.enabled = false;

            if (targetingPrefabInstance == null)
            {
                targetingPrefabInstance = Instantiate(targetingPrefab);
            }
            else
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }

            targetingPrefabInstance.localScale = new Vector3(areaAffectRadius * 2, 0, areaAffectRadius * 2);

            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

                RaycastHit raycastHit;
                if (Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        // Absorbs the mouse click to prevent unwanted movement
                        yield return new WaitWhile(() => Input.GetMouseButton(0));

                        playerController.enabled = true;
                        targetingPrefabInstance.gameObject.SetActive(false);
                        finshed(GetGameObjectsInRadius(raycastHit.point));
                        yield break;
                    }
                }

                yield return null;
            }
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);

            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}
