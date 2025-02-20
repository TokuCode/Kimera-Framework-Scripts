using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class AttackOnLand :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureAction, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public string combatCondition;
        public EntityAnimator animator;
        //References
        //Componentes

        private void Awake()
        {
            animator = gameObject.GetComponent<EntityAnimator>();
        }
        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            
            //Setup Properties

            ToggleActive(false);
        }

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            ToggleActive(true);
        
            if(settings.Length > 0)
            {
                combatCondition = settings[0].stringValue;
            }
        }   

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            TerrainEntity terrain = controller as TerrainEntity;

            if (terrain == null) return;

            if (terrain.onGround)
            {
                CombatAnimator combatAnimator = controller.SearchFeature<CombatAnimator>();

                combatAnimator.InputConditon("stop");
                if (animator != null) animator.FeatureAction(controller, new Setting("triggerName", "Cancel", Setting.ValueType.String));
                combatAnimator.InputConditon(combatCondition);

                ToggleActive(false);
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