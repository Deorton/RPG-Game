using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health Target = null;
        GameObject instigator = null;

        [SerializeField] float speed = 1f;
        [SerializeField] float projectileDamage = 10f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] UnityEvent onHit;

        float damageFromWeapon = 0f;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (Target == null) { return; }

            if (isHoming && !Target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            Target = target;
            this.instigator = instigator;
            damageFromWeapon = damage + projectileDamage;
            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = Target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) { return Target.transform.position; }

            return Target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != Target) { return; }

            if (Target.IsDead()) { return; }

            speed = 0;
            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            Target.TakeDamage(instigator, damageFromWeapon);
            
            if (GetComponent<DestroyAfterEffect>() != null)
            {
                GetComponent<DestroyAfterEffect>().DestroyNow();
            }
            
            Destroy(gameObject);
        }
    }
}
