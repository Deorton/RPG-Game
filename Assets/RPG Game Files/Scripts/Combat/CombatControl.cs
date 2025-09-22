using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
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
        [SerializeField] WeaponConfig defaultWeapon = null;
        
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
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
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();

            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
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

            if (!GetIsInRange(currentTarget.transform))
            {
                movementControl.MoveTo(currentTarget.transform.position, 1f);
            }
            else
            {
                movementControl.Stop();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(RightHandTransform, LeftHandTransform, animator);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(currentTarget.transform);

            if (timeSinceLastAttack > currentWeaponConfig.GetTimeBetweenAttacks())
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

        private bool GetIsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null) return false;
            if(!GetComponent<MovementControl>().CanMoveTo(target.transform.position) && !GetIsInRange(target.transform)) return false;
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
            if (currentTarget == null) return;

            float totalDamage = baseStats.GetStat(Stat.damage);
            
            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            currentTarget.TakeDamage(gameObject, totalDamage);
        }

        void Shoot()
        {
            if (currentTarget == null) return;
            
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(RightHandTransform, LeftHandTransform, currentTarget, gameObject, baseStats.GetStat(Stat.damage));
            }
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

         public JToken CaptureAsJToken()
         {
             return JToken.FromObject(currentWeaponConfig.name);
         }

         public void RestoreFromJToken(JToken state)
         {
             string weaponName = state.ToObject<string>();
             WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
             EquipWeapon(weapon);
         }
    }
}

