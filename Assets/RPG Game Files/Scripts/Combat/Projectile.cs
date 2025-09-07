using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health Target = null;
        [SerializeField] float speed = 1f;


        // Update is called once per frame
        void Update()
        {
            if (Target == null) { return; }

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target)
        {
            Target = target;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = Target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) { return Target.transform.position; }

            return Target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
    }
}
