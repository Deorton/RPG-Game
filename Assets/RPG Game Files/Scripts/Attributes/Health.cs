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
        bool isDead = false;

        void Start()
        {
            healthPoints = GetComponent<BaseStat>().getHealth();
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);

            if (healthPoints == 0)
            {
                Die();
            }
        }

        public float GetHealthPercentage()
        {
            return 100 * (healthPoints / GetComponent<BaseStat>().getHealth());
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
