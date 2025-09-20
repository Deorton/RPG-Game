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
    public class CombatControl : MonoBehaviour, IAction, IModifierProvider//,  IJsonSaveable
    {
        [SerializeField] Transform RightHandTransform = null;
        [SerializeField] Transform LeftHandTransform = null;
        [SerializeField] Weapon deafaultWeapon = null;
        

        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        Health currentTarget;
        MovementControl movementControl;
        BaseStats baseStats;
        Animator animator;

        void Awake()
        {
            movementControl = GetComponent<MovementControl>();
            baseStats = GetComponent<BaseStats>();
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

        public void EquipWeapon(Weapon weapon)
        {
            weapon.Spawn(RightHandTransform, LeftHandTransform, animator);
            currentWeapon = weapon;
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
            
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(RightHandTransform, LeftHandTransform, currentTarget, gameObject, baseStats.GetStat(Stat.damage));
            }
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            throw new NotImplementedException();
        }

        //  public JToken CaptureAsJToken()
        //  {
        //      return JToken.FromObject(currentWeaponConfig.name);
        //  }

        //  public void RestoreFromJToken(JToken state)
        //  {
        //      string weaponName = state.ToObject<string>();
        //      WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
        //      EquipWeapon(weapon);
        //  }
    }
}

