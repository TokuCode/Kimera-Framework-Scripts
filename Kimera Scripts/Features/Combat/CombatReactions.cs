using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class CombatReactions :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Controller controller;
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        [Header("Properties")]
        public float disableTimeAfterHit;
        //References
        [Header("References")]
        public Life life;
        public Stun stun;
        public MovementIntelligence movementIntel;
        public CombatAnimator combatAnimator;
        public EntityAnimator animator;
        //Componentes
        [Header("Components")]
        public CrowdIntelligence<Enemy> enemyCrowd;

        private void Awake()
        {
            //Get References
            life = GetComponent<Life>();
            stun = GetComponent<Stun>();
            combatAnimator = GetComponent<CombatAnimator>();
            if (movementIntel == null) movementIntel = GetComponent<MovementIntelligence>();
            animator = GetComponent<EntityAnimator>();
        }

        private void OnEnable()
        {
            if (life != null) life.OnDamage += ReactToDamage;
        }

        private void OnDisable()
        {
            if (life != null) life.OnDamage -= ReactToDamage;
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            this.controller = controller;

            //Setup Properties
            disableTimeAfterHit = settings.Search("disableTimeAfterHit");

            ToggleActive(true);
        }

        private void ReactToDamage()
        {
            if (!active) return;

            if (stun != null) stun.StunSomeTime(disableTimeAfterHit);

            //if (combatAnimator != null) combatAnimator.InputConditon("stop");

            if(animator != null) animator.FeatureAction(controller, new Setting("triggerName", "Hurt", Setting.ValueType.String));

            Enemy meEnemy = controller as Enemy;
            //if(enemyCrowd != null && life != null && meEnemy != null && movementIntel != null)
            //{
            //    if (life.CurrentHealth <= movementIntel.runAwayLife)
            //    {
            //        enemyCrowd.SetUnitOutOfBattle(meEnemy);
            //    }
            //}

            if(meEnemy != null) 
            {
                Camera_System.instance.CameraShake("DamageEnemy");
            }

            Player mePlayer = controller as Player;
            if(mePlayer != null)
            {
                if(mePlayer.block)
                {
                    Camera_System.instance.CameraShake("Block");
                }
                else
                {
                    Camera_System.instance.CameraShake("Damage");
                }                
            }
        }

        public void PassTurn()
        {
            if (!active) return;

            Enemy meEnemy = controller as Enemy;
            if (enemyCrowd != null && meEnemy != null)
            {
                enemyCrowd.SetUnitOutOfBattle(meEnemy);
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
    }
}