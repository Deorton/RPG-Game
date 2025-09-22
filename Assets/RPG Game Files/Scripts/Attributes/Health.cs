using System;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [Range(0, 100)]
        [SerializeField] float healthLevelUpPercentage = 70f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> healthPoints;
        float healthpointSaveValue = 0f;

        BaseStats baseStats;
        bool isDead = false;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        void Start()
        {
            healthPoints.ForceInit();
        }

        void OnEnable()
        {
            baseStats.onLevelUp += UpdateHealthOnLevelUp;
        }

        void OnDisable()
        {
            baseStats.onLevelUp -= UpdateHealthOnLevelUp;
        }

        private void UpdateHealthOnLevelUp()
        {
            float LevelledUpHealthPoints = baseStats.GetStat(Stat.Health) * (healthLevelUpPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, LevelledUpHealthPoints);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            //    print(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPercentage()
        {
            return 100 * GetHealthFraction();
        }

        public float GetHealthFraction()
        {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
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
            healthpointSaveValue = healthPoints.value;
            return JToken.FromObject(healthpointSaveValue);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthpointSaveValue = state.ToObject<float>();
            healthPoints.value = healthpointSaveValue;
            UpdateState();
        }

        private void UpdateState()
        {
            if (healthPoints.value <= 0)
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
