using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Gravity : MonoBehaviour, IActivable, IFeatureSetup, IFeatureFixedUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        [Header("Properties")]
        [HideInInspector]
        public Coroutine gravityCoroutine;
        public float gravityValue;
        public float maxVerticalSpeed;
        //References
        [Header("References")]
        [SerializeField] private List<TerrainModifier> terrains;
        [SerializeField] private Dash dash;
        //Componentes
        [Header("Components")]
        public Rigidbody cmp_rigidbody;

        private void Awake()
        {
            //Setup References
            dash = GetComponent<Dash>();
            terrains = new List<TerrainModifier>(GetComponents<TerrainModifier>());
            terrains.Sort(TerrainModifier.CompareByOrder);

            //Setup Components
            cmp_rigidbody = GetComponent<Rigidbody>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties
            gravityValue = settings.Search("gravityValue");
            maxVerticalSpeed = settings.Search("maxVerticalSpeed");

            ToggleActive(true);
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public void FixedUpdateFeature(Controller controller)
        {
            if (!active) return;

            LimitVerticalSpeed();

            TerrainEntity terrain = controller as TerrainEntity;
            if (terrain == null) return;

            SpecialTerrainEntity specialTerrain = controller as SpecialTerrainEntity;

            ApplyGravity(terrain, specialTerrain);
        }

        public void ApplyGravity(TerrainEntity terrain, SpecialTerrainEntity specialTerrin)
        {
            bool isDashing = dash != null ? dash.IsDashing && !dash.IsCharging : false;
            bool grounded = terrain.onGround;
            bool onSlope = terrain.onSlope;
            bool onLadder = specialTerrin != null ? specialTerrin.onLadder : false;

            if (isDashing || (grounded && !onSlope && !onLadder)) return;

            Vector3 direction = Vector3.up;

            terrains.Sort(TerrainModifier.CompareByOrder);
            if (terrains.Count > 0) if (terrains[terrains.Count - 1].OnTerrain) direction = terrains[terrains.Count - 1].GetTerrainNormal();

            cmp_rigidbody.AddForce(-direction * gravityValue * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        public void LimitVerticalSpeed()
        {
            if (Mathf.Abs(cmp_rigidbody.velocity.y) <= maxVerticalSpeed) return;

            cmp_rigidbody.velocity = new Vector3(cmp_rigidbody.velocity.x, maxVerticalSpeed * Mathf.Sign(cmp_rigidbody.velocity.y), cmp_rigidbody.velocity.z);
        }

        
        public IEnumerator ReturnGravity(float duration)
        {
            
            float t = 0;
            float targetGravity = settings.Search("gravityValue");
            //gravityValue = targetGravity;
           
            gravityValue = 0;
            while (true)
            {
                yield return null;
                
                //Debug.Log(duration.ToString()+ " / " + t.ToString() + " / " + gravityValue.ToString());
                t += Time.deltaTime;
                gravityValue = Mathf.Lerp(gravityValue, targetGravity, t / duration);                

                if (t > duration)
                {

                    gravityCoroutine = null;
                    break;

                }
            }
        }
    }
}

