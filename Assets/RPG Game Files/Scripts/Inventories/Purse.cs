using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float startingBalance = 500f;

        float balance = 0;

        public event Action onChange;

        void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;

            if (onChange != null)
            {
                onChange();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(balance);
        }

        public void RestoreFromJToken(JToken state)
        {
            balance = state.ToObject<float>();
        }

    }
}
