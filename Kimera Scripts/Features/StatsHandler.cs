using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class StatsHandler :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        private const int DEFAULT_START_ATTACK = 15;
        private const int DEFAULT_START_MAX_HEALTH = 100;
        private const float DEFAULT_START_ACCELERATION = 10f;
        private const float DEFAULT_START_MAX_SPEED = 15f;
        private const float DEFAULT_ATTACK_INCREMENT = .35f;
        private const float DEFAULT_MAX_HEALTH_INCREMENT = 1f;
        private const float DEFAULT_ACCELERATION_INCREMENT = .05f;
        private const float DEFAULT_MAX_SPEED_INCREMENT = .05f;
        private const int DEFAULT_STAT_STATES_COUNT = 6;

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private float furryState;
        //Properties
        [Header("Properties")]
        public int startAttack;
        public int startMaxHealth;
        public float startAcceleration;
        public float startMaxSpeed;
        public float attackIncrementPerFurry;
        public float maxHealthIncrementPerFurry;
        public float accelerationIncrementPerFurry;
        public float maxSpeedIncrementPerFurry;
        public int statStatesCount;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            var tempStartAttack = settings.Search("attack");
            var tempstartMaxHealth = settings.Search("maxHealth");
            var tempStartAcceleration = settings.Search("acceleration");
            var tempStartMaxSpeed = settings.Search("maxSpeed");
            var tempAttackIncrementPerFurry = settings.Search("attackIncrementPerFurry");
            var tempMaxHealthIncrementPerFurry = settings.Search("maxHealthIncrementPerFurry");
            var tempAccelerationIncrementPerFurry = settings.Search("accelerationIncrementPerFurry");
            var tempMaxSpeedIncrementPerFurry = settings.Search("maxSpeedIncrementPerFurry");
            var tempStatStatesCount = settings.Search("statStatesCount");

            if (tempStartAttack != null) startAttack = tempStartAttack;
            else startAttack = DEFAULT_START_ATTACK;

            if (tempstartMaxHealth != null) startMaxHealth = tempstartMaxHealth;
            else startMaxHealth = DEFAULT_START_MAX_HEALTH;

            if(tempStartAcceleration != null) startAcceleration = tempStartAcceleration;
            else startAcceleration = DEFAULT_START_ACCELERATION;

            if (tempStartMaxSpeed != null) startMaxSpeed = tempStartMaxSpeed;
            else startMaxSpeed = DEFAULT_START_MAX_SPEED;

            if (tempAttackIncrementPerFurry != null) attackIncrementPerFurry = tempAttackIncrementPerFurry;
            else attackIncrementPerFurry = DEFAULT_ATTACK_INCREMENT;

            if (tempMaxHealthIncrementPerFurry != null) maxHealthIncrementPerFurry = tempMaxHealthIncrementPerFurry;
            else maxHealthIncrementPerFurry = DEFAULT_MAX_HEALTH_INCREMENT;

            if (tempAccelerationIncrementPerFurry != null) accelerationIncrementPerFurry = tempAccelerationIncrementPerFurry;
            else accelerationIncrementPerFurry = DEFAULT_ACCELERATION_INCREMENT;

            if(tempMaxSpeedIncrementPerFurry != null) maxSpeedIncrementPerFurry = tempMaxSpeedIncrementPerFurry;
            else maxSpeedIncrementPerFurry= DEFAULT_MAX_SPEED_INCREMENT;

            if(tempStatStatesCount != null) statStatesCount = tempStatStatesCount;
            else statStatesCount = DEFAULT_STAT_STATES_COUNT;

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;
            
            FurryEntity furry = controller as FurryEntity;
            if(furry == null) return;

            float furryRatio = Mathf.Clamp01(furry.furryCount / furry.maxFurryCount);
            furryState = Mathf.Floor(furryRatio * (statStatesCount - 1));

            Combat combat = controller.SearchFeature<Combat>();
            Movement movement = controller.SearchFeature<Movement>();
            Life life = controller.SearchFeature<Life>();

            UpdateCombatAttack(combat, (furryState / (statStatesCount - 1)) * furry.maxFurryCount);
            UpdateMovementStats(movement, (furryState / (statStatesCount - 1)) * furry.maxFurryCount);
            UpdateLifeStats(life, (furryState / (statStatesCount - 1)) * furry.maxFurryCount);
        }

        private void UpdateCombatAttack(Combat combat, float furryCount)
        {
            if(combat == null) return;

            combat.SetAttack(startAttack + Mathf.FloorToInt(furryCount * attackIncrementPerFurry));
        }

        private void UpdateMovementStats(Movement movement, float furryCount)
        {
            if(movement == null) return;

            movement.SetAcceleration(startAcceleration + furryCount * accelerationIncrementPerFurry);
            movement.SetMaxSpeed(startMaxSpeed + furryCount * maxSpeedIncrementPerFurry);
        }

        private void UpdateLifeStats(Life life, float furryCount)
        {
            if(life == null) return;

            float actualMaxHealth = life.maxHealth;
            float newMaxHealth = startMaxHealth + furryCount * maxHealthIncrementPerFurry;

            if (actualMaxHealth == newMaxHealth) return;

            float maxHealthDiff = newMaxHealth - actualMaxHealth;

            life.MaxHealth(Mathf.FloorToInt(maxHealthDiff), true, false);
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