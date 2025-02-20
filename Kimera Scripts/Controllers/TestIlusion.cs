using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class TestIlusion : Controller, LivingEntity, KineticEntity, TerrainEntity, FollowEntity
    {

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

        //Follow
        public GameObject target { get; set; }

        private void OnEnable()
        {
            SearchFeature<Life>().OnDeath += OnDeath;            
        }

        private void OnDisable()
        {
            SearchFeature<Life>().OnDeath -= OnDeath;
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Period))
            {
                SpawnEnemy();
            }
        }*/

        public void SpawnEnemy()
        {
            SearchFeature<EnemySpawn>().SpawnEnemy(0, "TestEnemy", 5);
        }

        public void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}

