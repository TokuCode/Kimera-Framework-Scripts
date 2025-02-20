using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Wander :  MovementMode 
    {
        //Properties
        [Header("Properties Wander")]
        public float wanderDistance;
        [HideInInspector] public float wanderSpeed;

        public override void SetupFeature(Controller controller)
        {
            base.SetupFeature(controller);

            wanderSpeed = settings.Search("wanderSpeed");
            wanderDistance = settings.Search("wanderDistance");

            modeName = "wander";
            modeSpeed = wanderSpeed;
        }

        public override Vector3 RequestNextPoint(FollowEntity follow)
        {
            Vector3 wanderPosition;

            float iteration = 0;

            do
            {
                Vector2 randomVector = Random.insideUnitSphere.normalized * wanderDistance;

                Vector3 relativeWander = new Vector3(randomVector.x, 0f, randomVector.y);

                wanderPosition = relativeWander + transform.position;

                iteration++;
            } while (CheckBlocked(wanderPosition) && iteration < MAX_ITERATIONS);

            return iteration >= MAX_ITERATIONS ? transform.position : ToFloor(wanderPosition);
        }
    }
}