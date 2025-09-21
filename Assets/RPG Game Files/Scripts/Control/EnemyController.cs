using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float chaseRange = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        GameObject player;
        CombatControl combatControl;
        MovementControl movementControl;
        Health health;

        LazyValue<Vector3> startingPosition;
        LazyValue<Vector3> startingRotation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            combatControl = GetComponent<CombatControl>();
            movementControl = GetComponent<MovementControl>();
            health = GetComponent<Health>();

            startingPosition = new LazyValue<Vector3>(GetStartingPosition);
            startingRotation = new LazyValue<Vector3>(GetStartingRotation);
        }

        private Vector3 GetStartingRotation()
        {
            return transform.eulerAngles;
        }

        private Vector3 GetStartingPosition()
        {
            return transform.position;
        }

        void Start()
        {
            startingPosition.ForceInit();
            startingRotation.ForceInit();
        }

        void Update()
        {
            if (health.IsDead()) return;

            if (InAttackRangeOfPlayer() && combatControl.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                //suspicion state
                SuspicionBehaviour();
            }
            else if (patrolPath != null)
            {
                PatrolBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            UpdateTimers();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition;

            if (AtWaypoint())
            {
                timeSinceArrivedAtWaypoint = 0;
                CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();

            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                movementControl.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void GuardBehaviour()
        {
            movementControl.StartMoveAction(startingPosition.value, patrolSpeedFraction);
            
            if (Vector3.Distance(transform.position, startingPosition.value) < 0.1f)
            {
                if (Vector3.Distance(transform.eulerAngles, startingRotation.value) > 0.1f)
                {
                    //rotate back to starting rotation
                    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, startingRotation.value, Time.deltaTime * 10f);
                }
            }
            
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            combatControl.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseRange;
        }

        //called by Unity
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}
