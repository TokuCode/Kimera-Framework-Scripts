using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features
{
    public class CombatAnimator :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureAction, IFeatureUpdate, ICombatAnimator //Other channels
    {
        private const float DEFAULT_ANIMATION_CLIP_BUFFER_RATIO = .5f;

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        private Dictionary<string, bool> conditions;
        private Dictionary<string, Coroutine> coroutinesInput;
        [SerializeField] private string currentCondition;
        [SerializeField] private string lastCondition;
        [SerializeField] private float variableInputPermanenceTime;
        //Properties
        [Header("Properties")]
        public float inputPermanenceTime;
        public float animationClipBufferRatio;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            conditions = new Dictionary<string, bool>();
            coroutinesInput = new Dictionary<string, Coroutine>();
            conditions.Add("stop", false);
            coroutinesInput.Add("stop",null);

            //Setup Properties
            inputPermanenceTime = settings.Search("inputPermanenceTime");

            float? tempAnimationClipBufferRatio = settings.Search("animationClipBufferRatio");
            if (tempAnimationClipBufferRatio.HasValue) animationClipBufferRatio = tempAnimationClipBufferRatio.Value;
            else animationClipBufferRatio = DEFAULT_ANIMATION_CLIP_BUFFER_RATIO;

            string condition1 = settings.Search("combatCondition1");
            string condition2 = settings.Search("combatCondition2");
            string condition3 = settings.Search("combatCondition3");
            string condition4 = settings.Search("combatCondition4");
            string condition5 = settings.Search("combatCondition5");
            string condition6 = settings.Search("combatCondition6");

            if (condition1 != null) if (condition1 != string.Empty)
                {
                    conditions.Add(condition1, false);
                    coroutinesInput.Add(condition1, null);
                }
            if (condition2 != null) if (condition2 != string.Empty)
                {
                    conditions.Add(condition2, false);
                    coroutinesInput.Add(condition2, null);
                }
            if (condition3 != null) if (condition3 != string.Empty)
                {
                    conditions.Add(condition3, false);
                    coroutinesInput.Add(condition3, null);
                }
            if (condition4 != null) if (condition4 != string.Empty)
                {
                    conditions.Add(condition4, false);
                    coroutinesInput.Add(condition4, null);
                }
            if (condition5 != null) if (condition5 != string.Empty)
                {
                    conditions.Add(condition5, false);
                    coroutinesInput.Add(condition5, null);
                }
            if (condition6 != null) if (condition6 != string.Empty)
                {
                    conditions.Add(condition6, false);
                    coroutinesInput.Add(condition6, null);
                }

            ToggleActive(true);
        }

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            if (!active) return;

            if (settings.Length <= 0) return;

            try
            {
                string condition = settings[0].value as string;
                InputConditon(condition);
            }
            catch
            {
                Debug.LogError("The setting value must be a string");
            }
        }

        public void InputConditon(string condition)
        {
            if (!active) return;

            if (coroutinesInput.ContainsKey(condition))
            {
                lastCondition = condition;
                if (coroutinesInput[condition] != null) StopCoroutine(coroutinesInput[condition]);
                coroutinesInput[condition] = StartCoroutine(FlipFlopInputCondition(condition));
            }
        }

        public void CancelCondition(string condition){
            if (!active) return;

            if (coroutinesInput.ContainsKey(condition))
            {
                if (coroutinesInput[condition] != null) StopCoroutine(coroutinesInput[condition]);
                conditions[condition] = false;
            }
        }

        private IEnumerator FlipFlopInputCondition(string condition)
        {
            if (conditions.ContainsKey(condition))
            {
                conditions[condition] = true;
                yield return new WaitForSeconds(inputPermanenceTime + variableInputPermanenceTime);
                conditions[condition] = false;
            }
        }

        public List<string> GetActiveConditions()
        {
            List<string> activeCondtions = new List<string>();

            foreach (KeyValuePair<string, bool> condition in conditions)
            {
                if (condition.Value) activeCondtions.Add(condition.Key);
            }

            return activeCondtions;
        }

        public List<string> GetActiveLastCondition()
        {
            if (!conditions.ContainsKey(lastCondition)) return new List<string>();
            if (conditions[lastCondition] == false) return new List<string>();

            return new List<string>() { lastCondition };
        }

        public void SetVariableInputPermanenceTime(float animationClipLength)
        {
            variableInputPermanenceTime = animationClipLength * animationClipBufferRatio;
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public bool CheckCondition(string condition)
        {
            if(conditions.ContainsKey(condition))
            {
                return conditions[condition];
            }

            return false;
        }

        public void UpdateFeature(Controller controller)
        {
            currentCondition = ""; 
            GetActiveConditions().ForEach(x => currentCondition += $"{x} ");

            if (conditions.ContainsKey(lastCondition))
            {
                if (!conditions[lastCondition]) lastCondition = string.Empty;
            }
        }
    }
}