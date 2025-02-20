using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Slope : TerrainModifier
    {
        //States
        [Header("States")]
        public RaycastHit slopeHit;
        //Properties
        [Header("Properties")]
        public float extraDistanceSlope;
        public float maxAngleSlope; // 0 - 90 degrees
        //References
        //Componentes

        public override void SetupFeature(Controller controller)
        {
            base.SetupFeature(controller);

            //Other properties
            extraDistanceSlope = settings.Search("extraDistanceSlope");
            maxAngleSlope = settings.Search("maxAngleSlope");

            //Order
            terrainOrder = 2;
        }

        public override void CheckTerrain(Controller controller)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, extraDistanceSlope, terrainLayer))
            {
                float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

                onTerrain = slopeAngle != 0 && slopeAngle <= maxAngleSlope;
            }
            else
            {
                onTerrain = false;
            }

            TerrainEntity terrain = controller as TerrainEntity;

            if (terrain == null) return;

            terrain.onSlope = onTerrain;
        }

        public override Vector3 ProjectOnTerrain(Vector3 direction)
        {
            if (!onTerrain) return direction;
            if (slopeHit.normal == Vector3.zero) return direction;

            return Vector3.ProjectOnPlane(direction, slopeHit.normal);
        }

        public override Vector3 GetTerrainNormal()
        {
            if (!onTerrain) return Vector3.zero;

            return slopeHit.normal;
        }

        public bool IsSlopeSurface(Vector3 normal)
        {
            float slopeAngle = Vector3.Angle(normal, Vector3.up);

            return slopeAngle != 0 && slopeAngle <= maxAngleSlope;
        }
    }
}

