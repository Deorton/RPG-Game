using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class MovementControl : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            GetComponent<NavMeshAgent>().SetDestination(destination);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
    }
}
