using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Features
{
    public class CamControl :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        float camLerpDuration;
        float currentFury;
        bool midChange;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            camLerpDuration = settings.Search("camLerpDuration");

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
            FurryEntity furryEntity = controller as FurryEntity;

            currentFury = furryEntity.furryCount;

            ChangeFOV(currentFury);
        }

        void ChangeFOV(float fury)
        {
            float f = Mathf.InverseLerp(0, 100, fury);
            float fov = Mathf.Lerp(Camera_System.instance.defaultFOV, 100, f);
           
            if(Camera_System.instance.cmp_playerCamera.m_Lens.FieldOfView != fov)
            Camera_System.instance.FOVLerpVoid(fov, camLerpDuration);
        }
    }
}