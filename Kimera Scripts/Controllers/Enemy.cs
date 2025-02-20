using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Enemy : Controller, LivingEntity, KineticEntity, TerrainEntity, SpecialTerrainEntity, FollowEntity, CombatEntity
    {
        public const float DESPAWN_TIME = 5.5f;

        //Living
        public int currentHealth { get; set; }
        public int maxHealth { get; set; }

        //Kinetic
        public Vector3 speed { get; set; }
        public float currentSpeed { get; set; }
        public float maxSpeed { get; set; }

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
            if(SearchFeature<Life>()) SearchFeature<Life>().OnDeath += OnDeath;
            
        }

        private void OnDisable()
        {
            if (SearchFeature<Life>()) SearchFeature<Life>().OnDeath -= OnDeath;
        }

        private void Start()
        {
            AudioCaller audioCaller = GetComponent<AudioCaller>();

            if(audioCaller != null)
            {
                audioCaller.CallSound("Idle");
            }
        }

        public void OnDeath()
        {
            UpdateFeatures();
            CallFeature<Ragdoll>(new Setting("ragdollActivation", true, Setting.ValueType.Bool));
            SearchFeature<FillBarVisualizer>().ActiveVisuals(false);
            ToggleActive(false);
            Invoke("ReanimateAndSave", DESPAWN_TIME);
            this.enabled = false;

            SoundLibrary soundLibrary = GetComponent<SoundLibrary>();

            if (soundLibrary != null)
            {
                soundLibrary.CallAudioManager("Muerte");
            }
        }

        public void ReanimateAndSave()
        {
            this.enabled = true;
            ToggleActive(true);
            if (SearchFeature<FillBarVisualizer>()) SearchFeature<FillBarVisualizer>().ActiveVisuals(true);
            this.Setup();
            CallFeature<Ragdoll>(new Setting("ragdollActivation", false, Setting.ValueType.Bool));
            gameObject.SetActive(false);
        }
    }
}


