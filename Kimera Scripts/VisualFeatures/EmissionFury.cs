using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class EmissionFury :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public SkinnedMeshRenderer[] renderers;
        public float maxValue = 100;
        public float blendUnit;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties

            ToggleActive(true);
            /*FurryEntity furryEntity = controller as FurryEntity;
            blendUnit = maxValue / settings.Search("furryMax");*/

            //GroupEnabler(furryEntity.furryCount * blendUnit);
        }

        public void UpdateFeature(Controller controller)
        {
            
        }

        public void ChangeEmissionValue(float value, SkinMeshBlend blend)
        {
            
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