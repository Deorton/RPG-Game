using UnityEngine;
using System.Collections;

namespace RPG.Core
{
    public class csDestroyEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C))
            {
                if (targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                
                Destroy(gameObject);
            }
        }
    }
}
