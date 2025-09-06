using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public Vector3 GetWaypoint(int index)
        {
            return transform.GetChild(index).position;
        }

        public int GetNextIndex(int index)
        {
            if (index + 1 == transform.childCount)
            {
                return 0;
            }
            return index + 1;
        }

        public int GetRandomIndex()
        {
            return Random.Range(0, transform.childCount);
        }
    }
}
