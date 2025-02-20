using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Friction : MonoBehaviour, IActivable, IFeatureSetup, IFeatureFixedUpdate //Other channels
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
        public float groundFriction;
        public float airFriction;
        //References
        [Header("References")]
        [SerializeField] private List<TerrainModifier> terrains;
        //Componentes
        [Header("Components")]
        [SerializeField] private Rigidbody cmp_rigidbody;

        private void Awake()
        {
            //Setup References
            terrains = new List<TerrainModifier>(GetComponents<TerrainModifier>());
            terrains.Sort(TerrainModifier.CompareByOrder);

            //Setup Components
            cmp_rigidbody = GetComponent<Rigidbody>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            groundFriction = settings.Search("groundFriction");
            airFriction = settings.Search("airFriction");

            ToggleActive(true);
        }

        public void FixedUpdateFeature(Controller controller)
        {
            if (!active) return;

            InputEntity input = controller as InputEntity;
            if (input == null) return;

            TerrainEntity terrain = controller as TerrainEntity;
            if (terrain == null) return;

            SpecialTerrainEntity specialTerrainEntity = controller as SpecialTerrainEntity;

            ApplyFriction(input, terrain, specialTerrainEntity);
        }

        public void ApplyFriction(InputEntity input, TerrainEntity terrain, SpecialTerrainEntity specialTerrainEntity)
        {
            Vector2 inputDirection = input.inputDirection;
            Vector3 forward = input.playerForward;
            bool grounded = terrain.onGround;
            Vector3 velocity = cmp_rigidbody.velocity;

            terrains.Sort(TerrainModifier.CompareByOrder);
            if (terrains.Count > 0)
            {
                foreach(TerrainModifier terrainMod in terrains)
                {
                    if(!terrainMod.OnTerrain) continue;

                    forward = terrainMod.ProjectOnTerrain(forward);
                }
            }

            bool changeDir = ChangeDirection(forward, velocity);
            bool onLadder = specialTerrainEntity != null ? specialTerrainEntity.onLadder : false;
            if (!terrain.onSlope && !onLadder)velocity.y = 0f;

            float drag = 0f;
            if (grounded && !onLadder)
                drag = airFriction;
            else if (changeDir || inputDirection == Vector2.zero)
                drag = groundFriction;

            if (drag != 0) cmp_rigidbody.AddForce(-velocity * drag);

            bool ChangeDirection(Vector3 direction, Vector3 velocity)
            {
                if (direction == Vector3.zero) return false;
                return Vector3.Dot(direction, velocity) < 0;
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

