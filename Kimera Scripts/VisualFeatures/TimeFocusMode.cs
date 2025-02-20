using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class TimeFocusMode :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //Stats
        //Properties
        public bool instantModify;
        [Range(0.0f,1.0f)]
        public float duration;
        [Header("TimeScale")]
        public bool disableTimeScale;
        public float overrideTimeScale;
        [Header("FOV")]
        public float overrideFOV;
        [Header("CameraSpeed")]
        [Tooltip("Don´t forget the - to rest value")]
        public float speedModifier;
        private float previousSped;
        [Header("Target Lock")]
        public float overrideTargetDistance;
        public float overrideTargetRadius;
        public float overrideTargetAdjustSpeed;
        //References
        //Componentes
        public Camera_System camSys;

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

        public void EnableFocus()
        {
            if (camSys)
            {
                if (instantModify) camSys.SetFOV(overrideFOV);
                else camSys.FOVLerpInvoke(overrideFOV, duration);

                previousSped = camSys.ReadSens();
                camSys.UpdateSens(previousSped + speedModifier);
                camSys.gameData.sens = previousSped;
                camSys.gameData.Save();

                if (camSys.lockSys)
                {
                    camSys.lockSys.distance = overrideTargetDistance;
                    camSys.lockSys.detectRadius = overrideTargetRadius;
                    camSys.lockSys.adjustmentSpeed = overrideTargetAdjustSpeed;
                }
            }
            if(disableTimeScale==false)
            {
                if (instantModify) GameManager.SetTimeTo(overrideTimeScale);
                else GameManager.LerpTimeTo(overrideTimeScale, duration);

            }


        }

        public void DisableFocus()
        {
            if (camSys)
            {
                if (instantModify) camSys.ResetFOV();
                else camSys.ResetFOVLerp(duration);

                camSys.UpdateSens(camSys.gameData.sens);

                if (camSys.lockSys)
                {
                    camSys.lockSys.ResetValues();
                }
            }
            if (disableTimeScale==false)
            {
                if (instantModify) GameManager.ResetTime();
                else GameManager.LerpTimeTo(1, duration);
            }
        }
        public void ToggleActive(bool active)
        {
            this.active = active;
        }
    }
}