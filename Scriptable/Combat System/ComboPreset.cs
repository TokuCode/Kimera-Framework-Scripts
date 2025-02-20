using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    [CreateAssetMenu(fileName = "New Attack Preset", menuName = "Combat System/Combo Preset")]
    public class ComboPreset : ScriptableObject
    {
        [System.Serializable]
        public struct Interruption
        {
            public string condition;
            public ComboPreset nextCombo;
        }
        
        public AttackPreset[] attackChain;

        public string condition;

        public Interruption[] interruptions;
    }
}
