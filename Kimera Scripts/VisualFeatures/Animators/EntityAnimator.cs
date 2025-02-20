using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public abstract class EntityAnimator :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate, IFeatureAction //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        //References
        //Componentes
        [Header("Components")]
        public Animator cmp_animator;

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            SetAnimator(controller);
        }

        public abstract void SetAnimator(Controller controller);

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            if (!active) return;

            if (settings.Length < 1) return;

            string triggerName = settings[0].stringValue;

            if(string.IsNullOrEmpty(triggerName)) return;

            cmp_animator.SetTrigger(triggerName);
        }

        public void LockAnimator(bool value)
        {

            cmp_animator.SetBool("LOCK", value);

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