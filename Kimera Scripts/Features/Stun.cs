using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Features
{
    public class Stun : MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate, IFeatureAction //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private bool isStunned;
        public bool IsStunned { get => isStunned; }
        //States / Time Management
        [SerializeField] private float stunTimer;
        //Properties
        [Header("Properties")]
        //Properties / Time Management
        public float stunDuration;
        
        //References
        [Header("References")]
        [SerializeField] private List<ISubcontroller> subcontrollers;
        public EntityAnimator animator;
        //Componentes

        private void Awake()
        {
            //Setup References
            subcontrollers = new List<ISubcontroller>(GetComponents<ISubcontroller>());
            animator = GetComponent<EntityAnimator>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            stunDuration = settings.Search("stunDurationDefault");


            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if(!active) return;

            StunEntity stun = controller as StunEntity;
            if(stun != null) stun.isStunned = isStunned;

            if (stunTimer > 0) stunTimer -= Time.deltaTime;
            else if (isStunned) CleanseStun();
        }

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            if (settings.Length < 1)
            {
                StunSomeTime(stunDuration);
                return;
            }

            if (!float.TryParse(settings[0].value, out float stunTime))
            {
                StunSomeTime(stunDuration);
                return;
            }

            StunSomeTime(stunTime);
        }

        public void StunSomeTime(float stunTime)
        {
            if(!active) return;

            stunTimer = Mathf.Max(stunTimer, stunTime);

            if(isStunned) return;

            subcontrollers.ForEach(subcontroller => subcontroller.ToggleActiveSubcontroller(false));
            isStunned = true;
            if (animator != null) animator.cmp_animator.SetBool("Stun", true);
        }

        public void CleanseStun()
        {
            if(!active) return;

            stunTimer = 0f;

            if (!isStunned) return;

            subcontrollers.ForEach(subcontroller => subcontroller.ToggleActiveSubcontroller(true));
            isStunned = false;
            if (animator != null) animator.cmp_animator.SetBool("Stun", false);
        }

        public bool GetActive()
        {
            return active;
        }

        int activeCount = 0;
        public void ToggleActive(bool active)
        {
            //this.active = active;
            if (activeCount < 0)
            {
                activeCount = 0;
            }

            if (!active)
            {
                activeCount++;
            }
            else
            {
                activeCount--;
            }

            if (activeCount > 0)
            {
                this.active = false;
            }
            else
            {
                activeCount = 0;
                this.active = true;
            }

            if (!this.active) CleanseStun();
        }
    }
}

