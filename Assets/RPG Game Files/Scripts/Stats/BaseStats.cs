using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStat : MonoBehaviour
    {
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
    }
}
