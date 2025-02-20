using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Block :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate, IFeatureAction //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        public bool block;
        public bool parry;
        //States / Time Management
        private float parryTimer;
        [HideInInspector]
        public float parryCooldownTimer;
        //Properties
        [Header("Properties")]
        public float parryTime;
        public float parryCooldown;

        public float blockingDamageMultiplier;
        //References
        [Header("References")]
        public CombatAnimator combatAnimator;
        public IActivable movement, jump, rotation, combat, stun, dash, friction;
        public EntityAnimator animator;
        //Componentes
        Movement cmp_movement;
        Jump cmp_jump;
        Rotation cmp_rotation;
        Controller mainController;

        private void Awake()
        {
            combatAnimator = GetComponent<CombatAnimator>();
            movement = GetComponent<Movement>() as IActivable;
            jump = GetComponent<Jump>() as IActivable;
            rotation = GetComponent<Rotation>() as IActivable;
            stun = GetComponent<Stun>() as IActivable;
            combat = GetComponent<Combat>() as IActivable;
            dash = GetComponent<Dash>() as IActivable;
            friction = GetComponent<Friction>() as IActivable;

            cmp_movement = GetComponent<Movement>();
            cmp_jump = GetComponent<Jump>();
            cmp_rotation = GetComponent<Rotation>();
            animator = GetComponent<EntityAnimator>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            mainController = controller;
            //Setup Properties
            parryTime = settings.Search("parryTime");
            parryCooldown = settings.Search("parryCooldown");
            blockingDamageMultiplier = settings.Search("blockingDamageMultiplier");

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if(parryTimer > 0) parryTimer -= Time.deltaTime;
            else if(parryTimer <= 0 && parry == true)
            {
                parryTimer = 0;
                parry = false;
                InvokeEnd();
            }

            if (parryCooldownTimer > 0) parryCooldownTimer-= Time.deltaTime;
            else
            {
                parryCooldownTimer = 0;
            }

            if (!active) return;

            CombatEntity combat = controller as CombatEntity;
            if (combat != null)
            {
                combat.parry = parry;
                combat.block = block;
            }
        }

        public void StartBlock()
        {           
            if (parry == false && parryCooldownTimer <= 0)
            {
                
                //if (combat != null) combat.ToggleActiveSubcontroller(false);
                if (movement != null) movement.ToggleActive(false);
                cmp_movement.AttackFailsafe();
                if (rotation != null) rotation.ToggleActive(false);
                cmp_rotation.AttackFailsafe();
                if (jump != null) jump.ToggleActive(false);
                cmp_jump.AttackFailsafe();
                if (animator != null) animator.LockAnimator(true);
                if (stun != null) stun.ToggleActive(false);
                if (dash != null) dash.ToggleActive(false);

                CombatEntity combat = mainController as CombatEntity;
                if (combat != null)
                {
                    combat.parry = parry;
                    combat.block = block;
                }

                parry = true;
                block = true;
                parryTimer = parryTime;
                //combatAnimator.InputConditon("stop");
            }                
        }

        public void EndBlock()
        {   
            if(block)
            {
                block = false;
                InvokeEnd();
            }            
        }

        public void InvokeEnd()
        {
            if(!parry && !block)
            {
                if (animator != null) animator.LockAnimator(false);
                if (movement != null) movement.ToggleActive(true);                
                if (rotation != null) rotation.ToggleActive(true);                
                if (jump != null) jump.ToggleActive(true);                
                //if (combat != null) combat.ToggleActiveSubcontroller(true);
                if (stun != null) stun.ToggleActive(true);
                if (dash != null) dash.ToggleActive(true);
                if(friction != null) friction.ToggleActive(true);
                parryCooldownTimer = parryCooldown;
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

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            if (parryCooldownTimer > 0) return;
            if(settings.Length <= 0) return;

            bool value = settings[0].boolValue;

            if (value)
            {
                StartBlock();
                if (animator != null) animator.FeatureAction(controller, new Setting("triggerName", "Block", Setting.ValueType.String));
                return;
            }
            
            EndBlock();
        }
    }
}