using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features
{
    public class CombatAnimatorLinker :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        public const float GRACE_PERIOD = .1f;

        public enum BodyParts
        {
            LeftHand,
            RightHand,
            LeftElbow,
            RightElbow,
            LeftKnee,
            RightKnee,
            LeftFoot,
            RightFoot,
            Chest,
            Head,
            Unlink
        }

        [System.Serializable]
        public struct AnimationParentLink
        {
            public string guid;
            public BodyParts bodyPart;
            public Transform child;
            public Vector3 movement;
            public AnimationCurve movementCurve;
            public float duration;

            //State vars
            public float timer;
            public Vector3 offset;
        }

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private IDictionary<string, AnimationParentLink> links;
        [SerializeField] private List<AnimationParentLink> readLinks;
        //Properties
        //References
        //Componentes
        [Header("Components")]
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftElbow;
        [SerializeField] private Transform rightElbow;
        [SerializeField] private Transform leftKnee;
        [SerializeField] private Transform rightKnee;
        [SerializeField] private Transform leftFoot;
        [SerializeField] private Transform rightFoot;
        [SerializeField] private Transform chest;
        [SerializeField] private Transform head;

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            links = new Dictionary<string, AnimationParentLink>();

            ToggleActive(true);
        }

        public bool GetActive()
        {
            return active;
        }

        public void UpdateLink(AnimationParentLink link, string guid)
        {
            Transform bodyPart = GetAnimationBodyPart(link.bodyPart);

            link.timer += Time.deltaTime;

            if(link.timer > link.duration)
            {
                DestroyLink(link.guid);
                return;
            }

            Vector3 attackDirection = transform.forward * link.movement.z + transform.right * link.movement.x + transform.up * link.movement.y;
            link.offset += attackDirection * link.movementCurve.Evaluate(Mathf.Clamp01(link.timer / link.duration));

            link.child.position = bodyPart.position + bodyPart.transform.right * link.offset.x + bodyPart.transform.forward * link.offset.z + bodyPart.transform.up * link.offset.y;
            link.child.rotation = bodyPart.rotation;

            if (links.ContainsKey(guid)) links[guid] = link;
        }

        public string CreateLink(Transform target, BodyParts bodyPart, float duration, AnimationCurve movementCurve, Vector3 movementDirection, Vector3 startOffset)
        {
            AnimationParentLink link = new AnimationParentLink()
            {
                guid = System.Guid.NewGuid().ToString(),
                bodyPart = bodyPart,
                child = target,
                movement = movementDirection,
                movementCurve = movementCurve,
                duration = duration,
                timer = 0f,
                offset = startOffset
            };

            links.Add(link.guid, link);

            return link.guid;
        }

        public void DestroyLink(string guid)
        {
            if (!links.ContainsKey(guid)) return;
        
            links.Remove(guid);
        }

        private void FlushLinks()
        {
            links.Clear();
            readLinks.Clear();
        }

        public Transform GetAnimationBodyPart(BodyParts bodyPart)
        {
            switch(bodyPart)
            {
                case BodyParts.LeftHand:
                    return leftHand;

                case BodyParts.RightHand:
                    return rightHand;

                case BodyParts.LeftElbow:
                    return leftElbow;

                case BodyParts.RightElbow:
                    return rightElbow;

                case BodyParts.LeftKnee:
                    return leftKnee;

                case BodyParts.RightKnee:
                    return rightKnee;

                case BodyParts.LeftFoot:
                    return leftFoot;

                case BodyParts.RightFoot:
                    return rightFoot;

                case BodyParts.Chest:
                    return chest;

                case BodyParts.Head:
                    return head;
                
                default:
                    return transform;
            }
        }

        public void ToggleActive(bool active)
        {
            this.active = active;

            if(active == false) FlushLinks();
        }

        void IFeatureUpdate.UpdateFeature(Controller controller)
        {
            if (!active) return;

            readLinks = links.Values.ToList();

            links.Values.ToList().ForEach(link => UpdateLink(link, link.guid));
        }
    }
}