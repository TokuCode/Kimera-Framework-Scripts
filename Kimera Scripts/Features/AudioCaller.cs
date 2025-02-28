using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class AudioCaller :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
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

        public void CallSound(string name)
        {
            if(AudioManager.instance)
                AudioManager.instance.PlaySound(name);
        }

        public void StopSound(string name)
        {
            if (AudioManager.instance)
                AudioManager.instance.Stop(name);
        }
    }
}