using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEditor;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "Save1";
        [SerializeField] float fadeInTime = 0.2f;

        IEnumerator Start()
        {
            Fader fader = FindFirstObjectByType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        private void Delete()
        {
            GetComponent<JsonSavingSystem>().Delete("Save1");
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load("Save1");
        }

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save("Save1");
        }
    }
}
