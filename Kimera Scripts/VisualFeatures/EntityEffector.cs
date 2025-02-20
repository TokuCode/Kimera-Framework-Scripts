using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class EntityEffector : MonoBehaviour, IActivable, IFeatureSetup //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        [Header("SpawnFX")]
        public string spawnFxKey;
        public string hitFxKey;
        public string healFxKey;
        public string deadFxKey;
        [Header("MaterialAnimator")]
        public Animator[] lowHpGroup;
        public Animator[] shieldGroup;
        public Animator[] invulnerabilityGroup;
        //public string shieldBrkFxKey;

        private Controller ctrll;
        private bool contentAdded = false;
        //States
        //Properties
        //References
        //Componentes
        private void OnEnable()
        {
            AddEnableContent();
        }
        private void OnDisable()
        {
            if (ctrll == null) return;
            contentAdded = false;

            if (ctrll == null)
            {
                Debug.Log("Controler not detected");
                return;
            }
            if (ElementInstancer.instance != null)
            {
                if (ctrll.SearchFeature<Life>())
                {
                    Life lf = ctrll.SearchFeature<Life>();
                    if (hitFxKey != "") lf.OnDamage -= () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(hitFxKey), transform.position, transform);
                    if (healFxKey != "") lf.OnHeal -= () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(healFxKey), transform.position, transform);
                    if (deadFxKey != "") lf.OnDeath -= () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(deadFxKey), transform.position, transform);
                    if (deadFxKey != "") lf.OnExternalInvulnerability -= () => GroupAnimatorTrigger(invulnerabilityGroup, "Start");
                    if (deadFxKey != "") lf.OnExternalInvulnerabilityExit -= () => GroupAnimatorTrigger(invulnerabilityGroup, "End");
                }
            }

            if (ctrll.SearchFeature<Shield>())
            {
                Shield sh = ctrll.SearchFeature<Shield>();
                sh.OnShield -= () => GroupAnimatorTrigger(shieldGroup, "Start");
                sh.OnBroke -= () => GroupAnimatorTrigger(shieldGroup, "End");
            }

            if (ctrll.SearchFeature<Life>() && lowHpGroup.Length > 0)
            {
                Life lf = ctrll.SearchFeature<Life>();
                lf.OnLowVariance -= () =>
                {
                    if (lf.CurrentHealth < lf.lowHpIndicator) GroupAnimatorTrigger(shieldGroup, "Start");
                    else if (lf.CurrentHealth > lf.lowHpIndicator) GroupAnimatorTrigger(shieldGroup, "End");
                };
            }
        }
        public void SetupFeature(Controller controller)
        {
            this.enabled = false;
            settings = controller.settings;
            ctrll = controller;
            this.enabled = true;
            //Setup Properties
            if (contentAdded == false) AddEnableContent();
            ToggleActive(true);
        }
        public void AddEnableContent()
        {
            if (ctrll == null) return;
            contentAdded = true;

            Debug.Log("FisrtEnable");
            if (ctrll == null)
            {
                Debug.Log("Controler not detected");
                return;
            }

            if (ElementInstancer.instance != null)
            {
                Debug.Log("eeeeenable");
                if (spawnFxKey != "")
                {
                    GameObject tmpObj = ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(spawnFxKey), transform.position);
                    //tmpObj.transform.parent = null;
                }


                if (ctrll.SearchFeature<Life>())
                {
                    Life lf = ctrll.SearchFeature<Life>();

                    if (hitFxKey != "") lf.OnDamage += () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(hitFxKey), transform.position, transform);
                    if (healFxKey != "") lf.OnHeal += () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(healFxKey), transform.position, transform);
                    if (deadFxKey != "") lf.OnDeath += () => ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(deadFxKey), transform.position, transform);
                    if (deadFxKey != "") lf.OnExternalInvulnerability += () => GroupAnimatorTrigger(invulnerabilityGroup, "Start");
                    if (deadFxKey != "") lf.OnExternalInvulnerabilityExit += () => GroupAnimatorTrigger(invulnerabilityGroup, "End");
                }

            }

            if (ctrll.SearchFeature<Shield>() && shieldGroup.Length > 0)
            {
                Shield sh = ctrll.SearchFeature<Shield>();
                sh.OnShield += () => GroupAnimatorTrigger(shieldGroup, "Start");
                sh.OnBroke += () => GroupAnimatorTrigger(shieldGroup, "End");
            }

            if (ctrll.SearchFeature<Life>() && lowHpGroup.Length > 0)
            {
                Life lf = ctrll.SearchFeature<Life>();
                lf.OnLowVariance += () =>
                {
                    if (lf.CurrentHealth < lf.lowHpIndicator) GroupAnimatorTrigger(lowHpGroup, "Start");
                    else if (lf.CurrentHealth > lf.lowHpIndicator) GroupAnimatorTrigger(lowHpGroup, "End");
                };
            }
        }
        public void GroupAnimatorTrigger(Animator[] group, string triger)
        {
            foreach (Animator anim in group)
            {
                anim.SetTrigger(triger);
            }
        }
        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }
    }
}