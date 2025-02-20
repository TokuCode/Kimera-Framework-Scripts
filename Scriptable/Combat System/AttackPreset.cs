using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Features
{
    [CreateAssetMenu(fileName = "New Attack Preset", menuName = "Combat System/Attack Preset")]
    public class AttackPreset : ScriptableObject
    {
        public AnimationClip animationClipHuman;
        public AnimationClip animationClipBeast;
        public int attackToughness;


        [Header("Control")]
        public bool unallowPlayerControlOverride;

        public AttackSwing[] swings;


#if UNITY_EDITOR
        public void BuildAnimationEvents()
        {
            AnimationEvent[] clipEvents = new AnimationEvent[swings.Length * 2];

            for (int i = 0; i < swings.Length; i++)
            {
                AnimationEvent startEvent = new AnimationEvent();
                startEvent.functionName = "StartAttack";
                startEvent.time = Mathf.Clamp(swings[i].start, 0.01f, animationClipHuman.length-0.01f);
                startEvent.intParameter = i;

                AnimationEvent endEvent = new AnimationEvent();
                endEvent.functionName = "EndAttack";
                endEvent.time = Mathf.Clamp(swings[i].end, 0.01f, animationClipHuman.length - 0.01f);
                endEvent.intParameter = i;

                clipEvents[i * 2] = startEvent;
                clipEvents[i * 2 + 1] = endEvent;
            }

            AnimationUtility.SetAnimationEvents(animationClipHuman, clipEvents);
            AnimationUtility.SetAnimationEvents(animationClipBeast, clipEvents);
        }
#endif
    }
}
