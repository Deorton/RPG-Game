using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] float maxLifeTime = 10f;

        void Start()
        {
            Destroy(gameObject, maxLifeTime);
        }
        // Update is called once per frame
        void Update()
        {
            if (GetComponent<ParticleSystem>() != null)
            {
                if (!GetComponent<ParticleSystem>().IsAlive())
                {
                    Destroy(gameObject);
                }
            }
            else if (GetComponent<AudioSource>() != null)
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void DestroyNow()
        {
            Destroy(gameObject);
        }
    }
}
