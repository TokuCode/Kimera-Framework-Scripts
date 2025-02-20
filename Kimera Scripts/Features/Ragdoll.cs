using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
public class Ragdoll :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureAction //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        //References
        //Componentes
        public Animator cmp_anim;
        public GameObject ragdolModel;
        public Collider[] cmp_normalCols;
        public Rigidbody[] cmp_normalRB;
        private Collider[] cmp_ragdollCols;
        private Rigidbody[] cmp_ragdollRB;

        private void Awake()
        {
            if(cmp_anim==null)cmp_anim = GetComponent<Animator>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties

            cmp_ragdollCols = ragdolModel.GetComponentsInChildren<Collider>();
            cmp_ragdollRB = ragdolModel.GetComponentsInChildren<Rigidbody>();

            ToggleActive(true);
            RagdollSetActive(false);
        }

        public void RagdollSetActive(bool active)
        {
            if (ragdolModel == null) return;

            if (cmp_anim) cmp_anim.enabled = !active;

            foreach (Collider col in cmp_ragdollCols)
            {
                col.enabled = active;
            }
            foreach (Rigidbody rig in cmp_ragdollRB)
            {
                rig.isKinematic = !active;
            }

            foreach (Collider col in cmp_normalCols)
            {
                col.enabled = !active;
            }
            foreach (Rigidbody rig in cmp_normalRB)
            {
                rig.isKinematic = active;
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

        public void FeatureAction(Controller controller, params Setting[] settings)
        {
            //if(!active) return;

            if(settings.Length <= 0) return;

            RagdollSetActive(settings[0].boolValue);
        }
    }
}