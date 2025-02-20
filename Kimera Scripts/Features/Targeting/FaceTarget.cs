using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class FaceTarget :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public Rotation rotation;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            rotation = controller.SearchFeature<Rotation>();
            //Setup Properties

            ToggleActive(false);
        }

        public void UpdateFeature(Controller controller)
        {
            if (!active) return;

            FollowEntity follow = controller as FollowEntity;
            InputEntity input = controller as InputEntity;

            FaceTo(follow, input);
        }

        private void FaceTo(FollowEntity follow, InputEntity input)
        {
            if(follow == null) return;

            Vector3 directionToTarget = Vector3.zero;
            if (input.inputDirection != Vector2.zero && follow.target==null)
            {
                return;
                //directionToTarget = ProjectOnCameraFlattenPlane(new Vector3(input.inputDirection.x, 0f, input.inputDirection.y), input.playerCamera);
                //directionToTarget = ((directionToTarget * 2.5f) + transform.position);
            }
            else
            {
                if (follow.target == null) return;
                directionToTarget = follow.target.transform.position;
                directionToTarget.y = transform.position.y;              
            }

            /*if (rotation && rotation.GetActive()) rotation.RotateTo(directionToTarget);
            else*/ transform.LookAt(directionToTarget);

            if (input == null) return;

            input.playerForward = (directionToTarget - transform.position).normalized;
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }
        private Vector3 ProjectOnCameraFlattenPlane(Vector3 direction, Camera camera)
        {
            if (camera == null) return Vector3.zero;

            Vector3 cameraForward = camera.transform.forward;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);

            // Calculate the movement direction based on the camera's forward direction
            Vector3 projection = cameraForward * direction.x + cameraRight * direction.z;

            projection.y = 0;
            projection.Normalize();

            return projection;
        }
    }
}