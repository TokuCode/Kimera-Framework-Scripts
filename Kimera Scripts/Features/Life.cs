using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Features
{
    public class Life : MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Events
        public event Action OnDamage;
        public event Action OnHeal;
        public event Action OnDeath;
        public event Action OnLowVariance;
        public event Action OnExternalInvulnerability;
        public event Action OnExternalInvulnerabilityExit;

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        Controller cmp_Controller;
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private int currentHealth;
        public int CurrentHealth { get => currentHealth; }
        //Properties
        [Header("Properties")]
        public float lowHpIndicator;
        public int maxHealth;
        public float immunityDuration = 0.5f;
        public  bool isImmune = false;
        //References
        private Coroutine inmuneRutine;
        //Componentes

        public void SetupFeature(Controller controller)
        {
            cmp_Controller = controller;
            settings = controller.settings;

            maxHealth = settings.Search("maxHealth");
            currentHealth = maxHealth;

            ToggleActive(true);
        }

        public bool GetActive()
        {
            return active;
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            LivingEntity life = controller as LivingEntity;
            if (life != null)
            {
                life.currentHealth = currentHealth;
                life.maxHealth = maxHealth;
            }
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
        }

        public void Health(int amount, bool triggerEvents = true, bool ignoreBlock = false)
        {
            if (!active || amount == 0) return;

            int previousCurrentHealth = currentHealth;

            if(amount > 0)
            {
                currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

                if (cmp_Controller.SearchFeature<Shield>() && !ignoreBlock)
                {
                    if (previousCurrentHealth + amount > maxHealth)
                    {
                        cmp_Controller.SearchFeature<Shield>().ModifyShield((previousCurrentHealth + amount) - maxHealth);

                    }
                }
            }
            else
            {
                if (isImmune) return;
                if (cmp_Controller.SearchFeature<Shield>() && cmp_Controller.SearchFeature<Shield>().currentShield > 0 && !ignoreBlock)
                {
                    cmp_Controller.SearchFeature<Shield>().ModifyShield(amount);
                }
                else 
                {
                    
                    currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
                    if(cmp_Controller.SearchFeature<HudHandler>())
                    {
                        HUDController.instance.cmp_anim.SetTrigger("Damage");
                    }
                    
                }
                Inmunity(); //invulnerabilidad al tomar daño
            }           

            if (!triggerEvents) return;

            int diff = currentHealth - previousCurrentHealth;            

            if (diff > 0) OnHeal?.Invoke();
            else if (diff < 0) OnDamage?.Invoke();

            if (currentHealth <= 0) OnDeath?.Invoke();
            if ((previousCurrentHealth > lowHpIndicator && currentHealth < lowHpIndicator) || (previousCurrentHealth < lowHpIndicator && currentHealth > lowHpIndicator)) OnLowVariance.Invoke();
        }

        public void MaxHealth(int amount, bool readjust = true, bool triggerEvents = true)
        {
            if(!active || amount == 0) return;

            float previousMaxHealth = maxHealth;
            maxHealth = Mathf.Max(0, maxHealth + amount);

            if(readjust)
            {
                float readjustmentRatio = (previousMaxHealth != 0) ? currentHealth / previousMaxHealth : 0;
                int readJustHealth= (int)(maxHealth * readjustmentRatio);
                
                Health(readJustHealth - currentHealth, triggerEvents, true);
            }
        }

        public void HealthPercentual(int percentage, bool triggerEvents = true)
        {
            Health(PercentageToAmount(percentage), triggerEvents);
        }

        public void MaxHealthPercentual(int percentage, bool readjust = true, bool triggerEvents = true)
        {
            MaxHealth(PercentageToAmount(percentage), readjust, triggerEvents);
        }

        [ContextMenu("kill")]
        public void Kill()
        {
            currentHealth = 0;
            OnDeath?.Invoke();
        }
        public void Inmunity()
        {
            if (inmuneRutine != null) return;
            inmuneRutine = StartCoroutine(ImmunityCoroutine());
        }
        public IEnumerator ImmunityCoroutine()
        {
            isImmune = true;
            yield return new WaitForSeconds(immunityDuration);
            isImmune = false;
            inmuneRutine = null;
        }

        public IEnumerator ImmunityCoroutine(float time)
        {
            isImmune = true;
            OnExternalInvulnerability?.Invoke();
            yield return new WaitForSeconds(time);
            OnExternalInvulnerabilityExit?.Invoke();
            isImmune = false;
            inmuneRutine = null;
        }

        public void CallInmunity(float value)
        {
            if (inmuneRutine != null)
            {
                StopCoroutine(inmuneRutine);
                if(isImmune==true)
                {
                    OnExternalInvulnerabilityExit?.Invoke();
                    isImmune =false;
                }
            }

            inmuneRutine = StartCoroutine(ImmunityCoroutine(value));
        }
        private int PercentageToAmount(int percentage)
        {
            return (int)(maxHealth * Mathf.Clamp01((float)percentage / 100));
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }
    }
}

