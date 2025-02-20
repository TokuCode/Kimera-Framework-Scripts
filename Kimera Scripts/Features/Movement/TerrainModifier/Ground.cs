using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Features
{
    public class Ground : TerrainModifier
    {
        //States
        //Properties
        [Header("Properties")]
        public float extraDistanceGround;
        public Vector2 offsetGround;
        public Vector3 feetSize;
        public float delayDetection;
        private float delayTimer=0;
        //References
        //Componentes

        public override void SetupFeature(Controller controller)
        {
            base.SetupFeature(controller);

            //Other Properties
            extraDistanceGround = settings.Search("extraDistanceGround");
            offsetGround = settings.Search("offsetGround");
            feetSize = settings.Search("feetSize");

            //Order
            terrainOrder = 1;
        }

        public override void CheckTerrain(Controller controller)
        {
            Vector3 offset = new Vector3(offsetGround.x, extraDistanceGround, offsetGround.y);
            Vector3 origin = transform.position + offset - feetSize.y / 2 * transform.up;

            onTerrain = Physics.OverlapBox(origin, feetSize / 2, transform.rotation, terrainLayer).Length > 0;

            TerrainEntity terrain = controller as TerrainEntity;
            
            if (terrain == null) return;

            if (terrain.onGround == false &&  onTerrain==true) onTerrain = GetTerrainDelayed(onTerrain);

            terrain.onGround = onTerrain;
        }

        public bool GetTerrainDelayed(bool value)
        {
            
            if(value)
            {
                delayTimer = 0;
            }
            else
            {
                delayTimer += Time.deltaTime;
                if(delayTimer>=delayDetection)
                {
                    Debug.Log("terraintestcall | false");
                    return false;
                }
            }
            Debug.Log("terraintestcall | true");
            return true;
        }

        public override Vector3 ProjectOnTerrain(Vector3 direction)
        {
            Vector3 flattenDirection = direction;
            flattenDirection.y = 0f;

            return flattenDirection;
        }
            
        public override Vector3 GetTerrainNormal()
        {
            return Vector3.up;
        }
    }
}

