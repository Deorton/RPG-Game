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

        public override void StartTargetting(GameObject user)
        {
            PlayerController playerController = user.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(user, playerController));
        }

        private IEnumerator Targeting(GameObject user, PlayerController playerController)
        {
            playerController.enabled = false;

            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

                if (Input.GetMouseButtonDown(0))
                {
                    // Absorbs the mouse click to prevent unwanted movement
                    yield return new WaitWhile(() => Input.GetMouseButton(0));

                    playerController.enabled = true;
                    yield break;
                }

                yield return null;
            }
        }
    }
}
