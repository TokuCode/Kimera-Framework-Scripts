using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class ComboCounter :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [SerializeField] private int comboCount;
        [SerializeField] private int bloodyComboCount;
        //Properties
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties

            ToggleActive(true);
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            CombatEntity combat = controller as CombatEntity;
            FurryEntity furry = controller as FurryEntity;

            if (combat != null) comboCount = combat.comboCount;
            if (furry != null) bloodyComboCount = furry.furryCombo;
        }
    }
}