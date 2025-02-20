using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Features
{
    public class MovementModeSelector :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureFixedUpdate, ISubcontroller //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private MovementMode activeMoveMode;
        //Properties
        [Header("Properties")]
        public string defaultMoveMode;
        public float reachDestinationDistance;
        //References
        [Header("References")]
        public Dictionary<string, MovementMode> moveModes;
        public Rotation rotation;
        //Componentes
        [Header("Components")]
        public NavMeshAgent agent;
        public Rigidbody rb;

        private void Awake()
        {
            //Setup subcontroller
            if(rotation == null) rotation = GetComponent<Rotation>();

            //Setup components
            if(rb == null) rb = GetComponent<Rigidbody>();
            if (agent == null) agent = GetComponent<NavMeshAgent>();
        }

        public void SetupFeature(Controller controller)
        {
            //agent.enabled = false;
              
            settings = controller.settings;

            //Setup Properties
            defaultMoveMode = settings.Search("defaultMoveMode");
            reachDestinationDistance = settings.Search("reachDestinationDistance");

            ToggleActive(true);
        }

        private void OnEnable()
        {
            if (agent) StartCoroutine(FixAgent(agent));

        }
        private void Start()
        {
            //Setup Modes
            //agent.enabled = false;
            //agent.enabled = true;
            List<MovementMode> moveModesList = new List<MovementMode>(GetComponents<MovementMode>());
            moveModes = new Dictionary<string, MovementMode>();
            foreach (var moveMode in moveModesList)
            {
                moveModes.Add(moveMode.modeName, moveMode);
            }

            SetActiveMode(defaultMoveMode);
        }

        public void FixedUpdateFeature(Controller controller)
        {
            if(!active) return;

            FollowEntity follow = controller as FollowEntity;
            if(follow == null) return;

            MoveToMode(follow);
        }

        public void SetActiveMode(string mode)
        {
            if(!active || agent == null) return;

            if (!moveModes.ContainsKey(mode))
            {
                activeMoveMode = null;
                agent.destination = transform.position;
                agent.speed = 0f;
                return;
            }   

            activeMoveMode = moveModes[mode];
            agent.speed = activeMoveMode.modeSpeed;

            if (!agent.enabled) return;
            
            agent.destination = transform.position;
        }

        public void MoveToMode(FollowEntity follow)
        {
            if(!active || agent == null || activeMoveMode == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, agent.destination);

            if (distanceToTarget > reachDestinationDistance) return;

            agent.destination = activeMoveMode.RequestNextPoint(follow);
        }

        public string GetActiveMoveModeName()
        {
            if (activeMoveMode == null) return "quiet";
            else return activeMoveMode.modeName;
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;

            if (agent != null)
            {
                if (!active)
                {
                    if (agent.enabled)
                    {
                        agent.destination = transform.position;
                        agent.isStopped = true;
                        agent.enabled = false;
                    }
                } else
                {
                    agent.enabled = true;
                    agent.isStopped = false;
                }
            }

            if(rb == null) return;

            rb.isKinematic = active;
        }

        public void ToggleActiveSubcontroller(bool active)
        {
            moveModes.Values.ToList().ForEach(mode => mode.ToggleActive(active));
            if(rotation != null) rotation.ToggleActive(active);

            ToggleActive(active);
        }

        public IEnumerator FixAgent(NavMeshAgent agent)
        {
            agent.enabled = false;
            //yield return new WaitForSeconds(1);
            yield return null;

            agent.enabled = true;
        }
    }
}