using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Follow :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate, IFeatureAction //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("State")]
        [SerializeField] private bool targetDetected = false;
        //Properties
        [Header("Properties")]
        public float detectDistance;
        public string targetTag;
        //References
        //Componentes
        [Header("Components")]
        public GameObject targetGameObject;
        public CrowdIntelligence<Enemy> enemyCrowd;

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            detectDistance = settings.Search("detectDistance");
            targetTag = settings.Search("targetTag");

            targetGameObject = GameObject.FindGameObjectWithTag(targetTag);

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if(!active) return;

            FollowEntity follow = controller as FollowEntity;
            if(follow == null) return;

            DetectTarget(controller);
            UpdateTarget(follow);
        }

        private void DetectTarget(Controller controller)
        {
            if (!active || targetGameObject == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, targetGameObject.transform.position);

            if (distanceToTarget < detectDistance)
            {
                targetDetected = true;
                if(enemyCrowd != null)
                {
                    enemyCrowd.CrowdAlert();
                    Enemy meEnemy = controller as Enemy;
                    if (meEnemy != null) enemyCrowd.SetUnitConscious(meEnemy);
                }
            }
            else
            {
                Debug.Log("FARAWAT");
                if (enemyCrowd != null) return;
                Debug.Log("PASSaWAY");
                targetDetected = false;
            }
        }

        private void UpdateTarget(FollowEntity follow)
        {
            if (!active) return;

            if (!targetDetected)
            {
                follow.target = null;
                return;
            }

            follow.target = targetGameObject;
        }

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            if(settings.Length < 1) return;

            bool targetedState = settings[0].boolValue;

            targetDetected = targetedState;

            if (!targetedState) return;

            if (enemyCrowd != null)
            {
                Enemy meEnemy = controller as Enemy;
                if (meEnemy != null && !enemyCrowd.CrowdAlerted) enemyCrowd.SetUnitConscious(meEnemy);
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