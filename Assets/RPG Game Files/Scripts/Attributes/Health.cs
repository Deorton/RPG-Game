using System;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float healthPoints = 100f;

        BaseStats baseStats;
        bool isDead = false;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        void Start()
        {
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);

            if (healthPoints == 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetHealthPercentage()
        {
            return 100 * (healthPoints / baseStats.GetStat(Stat.Health));
        }

        private void Die()
        {
            if (!isDead)
            {
                GetComponent<Animator>().SetTrigger("Die");
                isDead = true;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null) return;

            experience.GainExperience(baseStats.GetStat(Stat.experienceReward));
            
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(healthPoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthPoints = state.ToObject<float>();
            UpdateState();
        }

        private void UpdateState()
        {
            if (healthPoints <= 0)
            {
                Die();
            }
            else
            {
                isDead = false;
                GetComponent<Animator>().ResetTrigger("Die");
            }
        }
    }
}
