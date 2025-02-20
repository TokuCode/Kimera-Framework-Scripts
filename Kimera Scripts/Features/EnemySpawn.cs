using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Features
{
    public class EnemySpawn :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
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

        public int poolIndex;
        public string enemyName;
        public int spawnIndex;
        public float distanceThreshold;
        public bool enemySpawned;
        public List<Transform> spawnPoints = new List<Transform>();
        [SerializeField]
        GameObject storedEnemy;

        void OnEnable()
        {
            if (SpawnManager.instance.currentModule != null)
            {
                foreach(Transform points in SpawnManager.instance.currentModule.subModule)
                {
                    spawnPoints.Add(points);
                }
            }
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            if(settings.Search("distanceThreshold") != null)
            {
                distanceThreshold = settings.Search("distanceThreshold");
            }

            if (settings.Search("poolIndex") != null) poolIndex = settings.Search("poolIndex");
            if (settings.Search("enemyName") != null) enemyName = settings.Search("enemyName");
            if (settings.Search("spawnIndex") != null) spawnIndex = settings.Search("spawnIndex");


            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active)
            {
                return;
            }

            FollowEntity followEntity = controller as FollowEntity;
            MeasureDistance(followEntity);

            if (storedEnemy != null)
            {
                if (storedEnemy.activeInHierarchy == false)
                {
                    storedEnemy = null;
                    enemySpawned = false;
                }
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

        void OnDisable()
        {
            spawnPoints.Clear();
            
        }

        public void SpawnEnemy(int poolIndex, string enemyName, int spawnIndex)
        {
            //Debug.Log(SpawnManager.instance + "SpawnMangerInstance");

            if (SpawnManager.instance.currentModule != null)
            {
                enemySpawned = true;
                storedEnemy = SpawnManager.instance.SpawnEnemySingle(poolIndex, enemyName, spawnPoints[spawnIndex]);                
            }
        }

        public void MeasureDistance(FollowEntity followEntity)
        {
            if(followEntity.target == null || enemySpawned == true)
            {
                //Debug.Log("Test");
                return;
            }

            Debug.Log(followEntity.target);
            float distance = Vector3.Distance(transform.position, followEntity.target.transform.position);

            if (distance < distanceThreshold) 
            {
                SpawnEnemy(poolIndex, enemyName, spawnIndex);
            }
        }
    }
}