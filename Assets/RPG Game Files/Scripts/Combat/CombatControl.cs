using System;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class CombatControl : MonoBehaviour, IAction
    {
        [SerializeField] Transform RightHandTransform = null;
        [SerializeField] Transform LeftHandTransform = null;
        [SerializeField] Weapon deafaultWeapon = null;
        

        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        Health currentTarget;
        MovementControl movementControl;
        Animator animator;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            EquipWeapon(deafaultWeapon);
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (currentTarget == null) return;

            if (currentTarget.IsDead()) return;

            if (!GetIsInRange())
            {
                movementControl.MoveTo(currentTarget.transform.position, 1f);
            }
            else
            {
                movementControl.Stop();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(currentTarget.transform);

            if (timeSinceLastAttack > currentWeapon.GetTimeBetweenAttacks())
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, currentTarget.transform.position) < currentWeapon.GetRange();
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null) return false;
            if (target.GetComponent<Health>().IsDead()) return false;

            return true;
        }

        public void Attack(GameObject target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            currentTarget = target.GetComponent<Health>();
        }

        public void CancelAttack()
        {
            StopAttack();
            currentTarget = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }

        public void Cancel()
        {
            CancelAttack();
        }

        // Animation Event
        void Hit()
        {
            if (currentTarget == null) return;
            currentTarget.TakeDamage(currentWeapon.GetDamage());
        }

        public void EquipWeapon(Weapon weapon)
        {
            weapon.Spawn(RightHandTransform, LeftHandTransform, animator);
            currentWeapon = weapon;
        }
    }
}

