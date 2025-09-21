using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class CombatControl : MonoBehaviour, IAction, IModifierProvider,  IJsonSaveable
    {
        [SerializeField] Transform RightHandTransform = null;
        [SerializeField] Transform LeftHandTransform = null;
        [SerializeField] Weapon deafaultWeapon = null;
        

        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon> currentWeapon;

        Health currentTarget;
        MovementControl movementControl;
        BaseStats baseStats;
        Animator animator;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(deafaultWeapon);
            return deafaultWeapon;
        }

        // Start is called before the first frame update
        void Start()
        {
            currentWeapon.ForceInit();
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

        public void EquipWeapon(Weapon weapon)
        {
            
            currentWeapon.value = weapon;
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(RightHandTransform, LeftHandTransform, animator);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(currentTarget.transform);

            if (timeSinceLastAttack > currentWeapon.value.GetTimeBetweenAttacks())
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
            return Vector3.Distance(transform.position, currentTarget.transform.position) < currentWeapon.value.GetRange();
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

        public Health GetTarget()
        {
            return currentTarget;
        }

        // Animation Event
        void Hit()
        {
            float totalDamage = baseStats.GetStat(Stat.damage);
            
            if (currentTarget == null) return;
            currentTarget.TakeDamage(gameObject, totalDamage);
        }

        void Shoot()
        {
            if (currentTarget == null) return;
            
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(RightHandTransform, LeftHandTransform, currentTarget, gameObject, baseStats.GetStat(Stat.damage));
            }
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

         public JToken CaptureAsJToken()
         {
             return JToken.FromObject(currentWeapon.value.name);
         }

         public void RestoreFromJToken(JToken state)
         {
             string weaponName = state.ToObject<string>();
             Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
             EquipWeapon(weapon);
         }
    }
}

