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

        private void Load()
        {
            GetComponent<JsonSavingSystem>().Load("Save1");
        }

        private void Save()
        {
            GetComponent<JsonSavingSystem>().Save("Save1");
        }
    }
}
