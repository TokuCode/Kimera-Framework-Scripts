using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace Features
{
    public class MovementIntelligence :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        public enum States
        {
            Idle,
            RunAway,
            Alert,
            Chase
        }

        public enum ActionState
        {
            Hostile,
            Available,
            OutOfBattle
        }

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private bool hostile;
        public bool Hostile { get => hostile; set => hostile = value; }
        [SerializeField] private bool available;
        public bool Available { get => available; }
        public States state;
        [SerializeField] private string moveMode;
        //Properties
        [Header("Properties")]
        //Properties / Idle
        public string idleMoveMode;
        //Properties / Run Away
        public string runAwayMoveMode;
        public int runAwayLife;
        public float runAwayDistance;
        public string outOfBattleMoveMode;
        public float repositionDistanceOutOfBattle;
        //Properties / Alert
        public string approachTargetMoveMode;
        public float alertDistance;
        public float repositionDistanceAlert;
        public string alertMoveMode;
        //Properties / Chase
        public string chaseMoveMode;
        public string combatCondition1;
        public float attackDistance;

        //References
        [Header("References")]
        [SerializeField] private MovementModeSelector movementMode;
        [SerializeField] private CombatAnimator combatAnim;
        //Componentes
        private Animator animator;
        private NavMeshAgent agent;

        private void Awake()
        {
            if(movementMode == null) movementMode = GetComponent<MovementModeSelector>();
            if(combatAnim == null) combatAnim = GetComponent<CombatAnimator>();
            agent = gameObject.GetComponent<NavMeshAgent>();
            animator = gameObject.GetComponent<Animator>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            idleMoveMode = settings.Search("idleMoveMode");
            runAwayMoveMode = settings.Search("runAwayMoveMode");
            runAwayLife = settings.Search("runAwayLife");
            runAwayDistance = settings.Search("runAwayDistance");
            outOfBattleMoveMode = settings.Search("outOfBattleMoveMode");
            approachTargetMoveMode = settings.Search("approachTargetMoveMode");
            alertDistance = settings.Search("alertDistance");
            repositionDistanceAlert = settings.Search("repositionDistanceAlert");
            alertMoveMode = settings.Search("alertMoveMode");
            chaseMoveMode = settings.Search("chaseMoveMode");
            combatCondition1 = settings.Search("combatCondition1");
            attackDistance = settings.Search("attackDistance");
            repositionDistanceOutOfBattle = settings.Search("repositionDistanceOutOfBattle");

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active || movementMode == null) return;

            FollowEntity follow = controller as FollowEntity;
            if(follow == null) return;

            StateMachine(follow);

            if (movementMode.GetActiveMoveModeName() != moveMode) movementMode.SetActiveMode(moveMode);
            
        }
        private void Update()
        {
            AnimSpeedLink();
        }
        private void AnimSpeedLink()
        {
            if (!animator) return;
            float velocity = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("MoveSpeedMulti",velocity);
        }

        private void StateMachine(FollowEntity follow)  
        {
            if(state == States.Idle)
            {
                StateIdle(follow);
            } 
            
            else if(state == States.RunAway)
            {
                StateRunAway(follow);
            }

            else if (state == States.Alert)
            {
                StateAlert(follow);
            }

            else if (state == States.Chase)
            {
                StateChase(follow);
            }

        }

        private void StateIdle(FollowEntity follow)
        {
            if(follow.target == null)
            {
                moveMode = idleMoveMode;
                return;
            }

            if(hostile) TransitionState(States.Chase);
            else if(available) TransitionState(States.Alert);
            else TransitionState(States.RunAway);
        }

        private void StateRunAway(FollowEntity follow)
        {
            if(follow.target == null)
            {
                TransitionState(States.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, follow.target.transform.position);

            if(distance < runAwayDistance)
            {
                moveMode = runAwayMoveMode;
                return;
            }

            if(distance > runAwayDistance + repositionDistanceOutOfBattle)
            {
                moveMode = approachTargetMoveMode;
                return;
            }

            moveMode = outOfBattleMoveMode;

            if (hostile) TransitionState(States.Chase);
            else if(available) TransitionState(States.Alert);
        }

        private void StateAlert(FollowEntity follow)
        {
            if (follow.target == null)
            {
                TransitionState(States.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, follow.target.transform.position);

            if (distance < alertDistance - repositionDistanceAlert)
            {
                moveMode = runAwayMoveMode;
                return;
            }

            if (distance > alertDistance + repositionDistanceAlert)
            {
                moveMode = approachTargetMoveMode;
                return;
            }

            moveMode = alertMoveMode;

            if (hostile) TransitionState(States.Chase);
            else if (!available) TransitionState(States.RunAway);
        }

        private void StateChase(FollowEntity follow)
        {
            if (follow.target == null)
            {
                TransitionState(States.Idle);
                return;
            }

            if (!hostile && !available)
            {
                TransitionState(States.RunAway);
                return;
            } else if (!hostile)
            {
                TransitionState(States.Alert);
                return;
            }

            float distance = Vector3.Distance(transform.position, follow.target.transform.position);

            if (distance < attackDistance)
            {
                if (combatAnim != null) combatAnim.InputConditon(combatCondition1);
                return;
            }

            moveMode = chaseMoveMode;
        }

        public void TransitionState(States state)
        {
            this.state = state;
        }

        public void SetActionState(ActionState actionState)
        {
            switch(actionState)
            {
                case ActionState.Hostile:
                    if (!available) return;
                    
                    hostile = true;
                    break;

                case ActionState.Available:
                    hostile = false;
                    available = true;
                    break;

                case ActionState.OutOfBattle:
                    hostile = false;
                    available = false; 
                    break;
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