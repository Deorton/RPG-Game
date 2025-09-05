using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class CombatControl : MonoBehaviour, IAction
    {
        [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damage = 10f;
        float timeSinceLastAttack = 0f;

        Health currentTarget;
        MovementControl movementControl;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (currentTarget == null) return;

            if (!GetIsInRange())
            {
                movementControl.MoveTo(currentTarget.transform.position);
            }
            else
            {
                movementControl.Stop();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0f;
            }
            
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange;
        }

        public void Attack(Health target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            currentTarget = target;
        }

        public void CancelAttack()
        {
            currentTarget = null;
        }

        public void Cancel()
        {
            CancelAttack();
        }

        // Animation Event
        void Hit()
        {
           currentTarget.TakeDamage(damage);
        }
    }
}

