using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    [System.Serializable]
    public class AttackSwing
    {
        public string swingName;

        [Header("Hitbox Settings")]
        public Vector3 size;
        public Vector3 offset;
        public Vector3 movement;

        [Header("Linker Settings")]
        public CombatAnimatorLinker.BodyParts bodyPart;
        public Vector3 movementLinker;
        public AnimationCurve movementCurve;

        [Header("Effects")]
        public Settings settings;

        [Header("Timing")]
        [HideInInspector] public float duration;
        public float start;
        public float end;

        

        [Header("Visual FX")]
        public bool hitboxAttach;
        public string vfxNames; 
    }
}
