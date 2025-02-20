using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Furry : MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        private const float DEFAULT_FURRY_EXTENSION_MAX = 3f;

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] public float furryCount;
        [SerializeField] private float lastFurryPunchTime;
        [SerializeField] private float furryExtensionActual;
        //Properties
        [Header("Properties")]
        public float furryIncrement;
        public float furryDecrement;
        public float furryMax;
        //Properties / Time Management
        public float furryExtensionBase;
        public float furryExtensionMax;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            furryIncrement = settings.Search("furryIncrement");
            furryDecrement = settings.Search("furryDecrement");
            furryMax = settings.Search("furryMax");
            furryExtensionBase = settings.Search("furryExtension");
            float? tempFurryExtensionMax = settings.Search("furryExtensionMax");

            if (tempFurryExtensionMax.HasValue) furryExtensionMax = tempFurryExtensionMax.Value;
            else furryExtensionMax = DEFAULT_FURRY_EXTENSION_MAX;

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            FurryEntity furry = controller as FurryEntity;

            if (furry != null) furry.maxFurryCount = furryMax;

            if (!active)
            {
                if(furry != null) furry.furryCount = 0;
                return;
            }

            if (furry != null)
            {
                furry.furryCount = furryCount;
                UpdateFurryExtension(furry);
            }

            if (lastFurryPunchTime + furryExtensionActual < Time.time)
            {
                DecreaseFurryCount();
                if (furry != null) furry.furryCombo = 0;
            }
        }

        private void UpdateFurryExtension(FurryEntity furry)
        {
            if (furry == null) return;

            furryExtensionActual = furryExtensionBase + furryExtensionMax * (furry.maxFurryCount / (furry.maxFurryCount + furry.furryCount));
        }

        public void IncreaseFurryCount()
        {
            if(!active) return;

            furryCount += furryIncrement;
            furryCount = Mathf.Clamp(furryCount, 0, furryMax);
            lastFurryPunchTime = Time.time;

            
        }

        private void DecreaseFurryCount()
        {
            furryCount -= furryDecrement * Time.deltaTime;
            furryCount = Mathf.Clamp(furryCount, 0, furryMax);

            
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;

            if (active) return;

            furryCount = 0;
        }
    }
}

