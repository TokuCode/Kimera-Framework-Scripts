using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace Features
{
    public class HeadRotation :  MonoBehaviour, IActivable, IFeatureSetup,IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("Properties")]
        [SerializeField] float angleMin;
        [SerializeField] float radiusDetectTarget;
        [SerializeField] AimConstraint aimConstraint;
        [SerializeField] LayerMask maskTarget;
        [SerializeField] List<GameObject> currentEnemies;
        [SerializeField] GameObject currentTarget;
        ConstraintSource currentConstraint;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            //angleMin = settings.Search("angleMin");
            ToggleActive(true);
        }

        public void RotateHead()
        {
            currentTarget = DetectEnemy();
            if(currentTarget != null)
            {
                currentConstraint = new ConstraintSource { sourceTransform = currentTarget.transform, weight = 1f };
                float angleTarget = Vector3.Angle(transform.forward,currentTarget.transform.position-transform.position);
                
                if(angleTarget < angleMin)
                {    
                    if (aimConstraint.sourceCount == 0)
                    aimConstraint.AddSource(currentConstraint);
                }
                else
                {
                    currentTarget = null;
                    if (aimConstraint.sourceCount != 0)
                    aimConstraint.RemoveSource(0);
                }
            }
            else
            {
                if (aimConstraint.sourceCount != 0)
                aimConstraint.RemoveSource(0);
            }
        }

        public GameObject DetectEnemy()
        {
            Collider[] enemyTarget = Physics.OverlapSphere(transform.transform.position, radiusDetectTarget, maskTarget);
            
            if(enemyTarget.Length == 0 ) return null;
            
            foreach (var target1 in enemyTarget)
            {
                float distance1 = (target1.transform.position -transform.position).magnitude;
                if(distance1 < radiusDetectTarget)
                {
                    if(!currentEnemies.Contains(target1.gameObject))
                    currentEnemies.Add(target1.gameObject);
                }
            }

            foreach (GameObject target in currentEnemies)
            {
                float distance = (target.transform.position -transform.position).magnitude;
                if(distance < radiusDetectTarget)
                {
                    float angleTarget = Vector3.Angle(transform.forward,target.transform.position-transform.position);
                    if(angleTarget < angleMin)
                    {
                        if (aimConstraint.sourceCount != 0)
                        aimConstraint.RemoveSource(0);
                        
                        return target;
                    }
                    
                }
            }
            return null;
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public void UpdateFeature(Controller controller)
        {
            RotateHead();
        }
    }
}