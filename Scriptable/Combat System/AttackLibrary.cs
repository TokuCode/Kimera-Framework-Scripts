using System;
using System.Collections;
using System.Collections.Generic;
using Features;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackLibrary", menuName = "Combat System/Attack Library")]
public class AttackLibrary : ScriptableObject
{
    [Serializable]
    public struct AttackFile
    {
        public string key;
        public Features.AttackPreset attack;
    }
    
    [Serializable]
    public struct ComboFile
    {
        public string key;
        public ComboPreset combo;
    }
    
    public List<AttackFile> attackFiles = new List<AttackFile>();
    public List<ComboFile> comboFiles = new List<ComboFile>();
}
