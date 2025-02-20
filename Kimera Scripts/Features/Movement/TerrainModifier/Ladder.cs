using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Ladder : TerrainModifier, IFeatureUpdate
    {
        //States
        [Header("States")]
        public RaycastHit stepHit;
        [SerializeField] private bool hipBlocked;
        [SerializeField] private bool feetBlocked;
        //States / Time Management
        private float stepTimer;
        //Properties
        [Header("Properties")]
        public float stepDistance;
        public float stepHeight;
        public float stepDepth;
        public float hipHeight;
        public Vector3 feetSize;
        public Vector3 hipSize;
        public AnimationCurve stepCurve;
        //Properties / Time Management
        public float timeToStep;
        //References
        [Header("References")]
        public Slope slope;
        //Componentes
        //Debug
        [Header("Debug")]
        public bool debug;

        private void Awake()
        {
            slope = GetComponent<Slope>();
        }

        public override void SetupFeature(Controller controller)
        {
            base.SetupFeature(controller);

            //Other properties
            stepDistance = settings.Search("stepDistance");
            stepHeight = settings.Search("stepHeight");
            stepDepth = settings.Search("stepDepth");
            feetSize = settings.Search("feetSize");
            hipHeight = settings.Search("hipHeight");
            hipSize = settings.Search("hipSize");
            stepCurve = settings.Search("stepCurve");
            timeToStep = settings.Search("timeToStep");

            //Order
            terrainOrder = 3;
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active)
            {
                stepTimer = 0;
                return;
            }

            if (onTerrain) stepTimer += Time.deltaTime;
            else if(stepTimer > 0) stepTimer -= Time.deltaTime;
        }

        public override void CheckTerrain(Controller controller)
        {
            InputEntity input = controller as InputEntity;
            if (input == null) return;

            Vector3 direction = transform.forward;
            Vector3 originStep = transform.position + transform.up * stepHeight + direction * stepDepth;
            Vector3 originHip = transform.position + transform.up * hipHeight;

            Collider[] hipCollisions = Physics.OverlapBox(originHip, hipSize, transform.rotation, terrainLayer);

            hipBlocked = hipCollisions.Length > 0;
            feetBlocked = Physics.BoxCast(originStep, feetSize / 2, direction, out stepHit, transform.rotation, stepDistance, terrainLayer);

            if (hipBlocked)
            {
                onTerrain = false;
            }
            else if(feetBlocked)
            {
                onTerrain = slope == null ? true : !slope.IsSlopeSurface(stepHit.normal);
            } else
            {
                onTerrain = false;
            }

            SpecialTerrainEntity specialTerrain = controller as SpecialTerrainEntity;

            if (specialTerrain == null) return;

            specialTerrain.onLadder = onTerrain;
        }

        public override Vector3 ProjectOnTerrain(Vector3 direction)
        {
            if (!onTerrain) return direction;
            if (stepHit.normal == Vector3.zero) return direction;

            Vector3 flattenDirection = new Vector3(direction.x, 0, direction.z);

            float value = Mathf.Clamp01(stepTimer / timeToStep);
            float stepInterpolation = stepCurve.Evaluate(value);

            Vector3 projection = flattenDirection.normalized * (1 - stepInterpolation) + Vector3.up * stepInterpolation;

            return projection.normalized;
        }

        public override Vector3 GetTerrainNormal()
        {
            if(!onTerrain) return Vector3.zero;

            return stepHit.normal;
        }

        private void OnDrawGizmos()
        {
            if (!debug || !Application.isPlaying) return;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = hipBlocked ? Color.red : Color.green;
            Vector3 originHip = Vector3.up * hipHeight;
            Gizmos.DrawCube(originHip, hipSize);

            Gizmos.color = feetBlocked ? Color.red : Color.green;
            Vector3 originFeet = Vector3.up * stepHeight + Vector3.forward * stepDepth;
            Vector3 endFeet = originFeet + Vector3.forward * stepDistance;
            Gizmos.DrawCube(originFeet, feetSize);
            Gizmos.DrawLine(originFeet, endFeet);

        }
    }
}
