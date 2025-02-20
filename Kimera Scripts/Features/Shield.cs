using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Shield :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        public Action OnBroke;
        public Action OnShield;
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public int maxShield;
        public int currentShield;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            maxShield = settings.Search("maxShield");

            //Setup Properties

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (currentShield > maxShield)
            {
                currentShield = maxShield;
            }
            else if (currentShield < 0) 
            {
                currentShield = 0;
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

        public void ModifyShield(int amount)
        {
            if (!active || amount == 0) return;

            int previousCurrentHealth = currentShield;
            currentShield = Mathf.Clamp(currentShield + amount, 0, maxShield);
            if (previousCurrentHealth <= 0 && currentShield > 0) OnShield.Invoke();
            if (currentShield <= 0) OnBroke.Invoke();
        }

        public void HalfShield()
        {
            ModifyShield(maxShield / 2);
        }
    }
}