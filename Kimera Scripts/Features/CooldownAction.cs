using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Features
{
    public class CooldownAction : MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate
    {
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        Controller cmp_Controller;
        [SerializeField] private bool active;
        //States
        [SerializeField]
        CooldownVerifier[] cooldowns;
        private float cooldownsInProgress = 0;
        //Properties


        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            for (int i = 0; i < cooldowns.Length; i++)
            {
                cooldowns[i].timer = cooldowns[i].cooldownTime;
                cooldowns[i].readyToCall = true;
            }

            cooldownsInProgress = 0;
            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            if (cooldownsInProgress <= 0) return;

            for (int i = 0; i < cooldowns.Length; i++)
            {
                if(cooldowns[i].readyToCall==false)
                {
                    cooldowns[i].timer += Time.deltaTime;
                    if(cooldowns[i].timer>= cooldowns[i].cooldownTime)
                    {
                        cooldowns[i].readyToCall=true;
                        cooldownsInProgress--;
                    }
                }
            }
        }

        public void VerifyToActivate(int cooldownNumberList, Action callback )
        {
            if (cooldowns.Length <= cooldownNumberList || cooldownNumberList<0) return;
            if (cooldowns[cooldownNumberList].readyToCall==true)
            {
                callback.Invoke();
                ResetCooldown(cooldownNumberList);

            }
        }

        public int GetCooldowntListValue(string searchName)
        {
            for (int i = 0; i < cooldowns.Length; i++)
            {
                if (cooldowns[i].keyName == searchName)
                {
                    return i;
                }
            }
            return -1;
        }

        public void ResetCooldown(int cooldownNumber)
        {
            if (cooldowns.Length <= cooldownNumber || cooldownNumber < 0) return;
            cooldowns[cooldownNumber].readyToCall = false;
            cooldowns[cooldownNumber].timer = 0;
            cooldownsInProgress++;

        }
        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public bool GetActive()
        {
            return active;
        }
    }
}
[System.Serializable]
public class CooldownVerifier
{
    public string keyName;
    [ReadOnly]
    public bool readyToCall;
    public float cooldownTime;

    [ReadOnly]
    public float timer;
}
