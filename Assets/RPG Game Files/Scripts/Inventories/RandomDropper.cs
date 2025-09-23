using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        //Config Data
        [Tooltip("The maximum distance scattered away from the dropper")]
        [SerializeField] float scatterDistance = 1.5f;
        [SerializeField] InventoryItem[] dropLibrary;
        [SerializeField] int numberOfDrops = 2;

        //CONSTANTS
        const int ATTEMPTS = 30;

        public void RandomLoot()
        {
            for (int i = 0; i < numberOfDrops; i++)
            {
                var item = dropLibrary[Random.Range(0, dropLibrary.Length)];
            DropItem(item, 1);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randompoint = transform.position + Random.insideUnitSphere * scatterDistance;
                if (NavMesh.SamplePosition(randompoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}
