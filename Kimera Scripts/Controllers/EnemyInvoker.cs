using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class EnemyInvoker : Controller, LivingEntity, KineticEntity, TerrainEntity, SpecialTerrainEntity, FollowEntity, CombatEntity
    {
        public const float DESPAWN_TIME = 5.5f;
        //Living
        public int currentHealth { get; set; }
        public int maxHealth { get; set; }

        //Kinetic
        public Vector3 speed { get; set; }
        public float maxSpeed { get; set; }
        public float currentSpeed { get; set; }

        //Terrain
        public bool onGround { get; set; }
        public bool onSlope { get; set; }

        public bool onLadder { get; set; }

        //Follow
        public GameObject target {  get; set; }
        //Combat
        public bool block { get; set; }
        public bool parry { get; set; }
        public int attack {  get; set; }
        public int comboCount { get; set; }

        private void OnEnable()
        {
            SearchFeature<Life>().OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            SearchFeature<Life>().OnDeath -= OnDeath;
        }

        public void OnDeath()
        {

            UpdateFeatures();
            CallFeature<Ragdoll>(new Setting("ragdollActivation", true, Setting.ValueType.Bool));
            SearchFeature<FillBarVisualizer>().ActiveVisuals(false);
            ToggleActive(false);
            Invoke("ReanimateAndSave", DESPAWN_TIME);

            SoundLibrary soundLibrary = GetComponent<SoundLibrary>();

            if (soundLibrary != null)
            {
                soundLibrary.CallAudioManager("Muerte");
            }
        }

        public void ReanimateAndSave()
        {
            ToggleActive(true);
            CallFeature<Ragdoll>(new Setting("ragdollActivation", false, Setting.ValueType.Bool));
            SearchFeature<FillBarVisualizer>().ActiveVisuals(true);

            this.Setup();
            gameObject.SetActive(false);
        }
    }
}


