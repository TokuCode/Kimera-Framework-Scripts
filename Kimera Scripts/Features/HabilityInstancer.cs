using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Features
{
    public class HabilityInstancer : MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;

        public SpawnManager spawnManager;
        public string instanciatorKey;
        private NavMeshAgent agent;
        private MovementIntelligence inteligence;
        //States
        //Propertiespub
        public float initCounter;
        public float habDuration;
        public float cooldown;

        private float initTime;
        public bool instantiateOnFirstActive;
        private float habTime;
        private float cooldownTime;
        private EntityAnimator animator;
        //References
        //Componentes
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<EntityAnimator>();
        }
        public void SetupFeature(Controller controller)
        {
            spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            inteligence = controller.SearchFeature<MovementIntelligence>();
            ToggleActive(true); 
            if(instantiateOnFirstActive) cooldownTime=cooldown;
        }
        public bool GetActive()
        {
            return active;
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;
            if (spawnManager == null) return;
            if (spawnManager.remainingEnemies.Count <= 0) return;

            

            if (inteligence != null)
            {
                if(inteligence.state != MovementIntelligence.States.Idle)
                {
                    initCounter = 0;
                    return;
                }
            }

            if(initTime>= Mathf.Abs(initCounter))
            {
                habTime += Time.deltaTime;
                if(cooldownTime>= Mathf.Abs(cooldown))
                {
                    habTime = 0;
                    cooldownTime = 0;
                    agent.enabled = false;
                    animator.cmp_animator.SetTrigger("Invoking");
                }

                if(habTime>=Mathf.Abs(habDuration))
                {
                    cooldownTime += Time.deltaTime;
                    agent.enabled = true;
                }

            }
            else
            {
                initTime += Time.deltaTime;
            }

        }

        public void Generate()
        {
            if (!active) return;
            if (spawnManager == null) return;
            if (spawnManager.remainingEnemies.Count <= 0) return;

            ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(instanciatorKey), spawnManager.remainingEnemies[Random.Range(0, spawnManager.remainingEnemies.Count-1)].transform.position);
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }
    }
}
