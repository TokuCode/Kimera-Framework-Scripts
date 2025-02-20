using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public abstract class TerrainModifier : MonoBehaviour, IActivable, IFeatureSetup, IFeatureFixedUpdate
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] protected bool active;
        //States
        [Header("States")]
        [SerializeField] protected bool onTerrain;
        public bool OnTerrain { get { return onTerrain && active; } }
        public int terrainOrder;
        //Properties
        [Header("Properties")]
        public LayerMask terrainLayer;
        //References
        //Componentes

        public virtual void SetupFeature(Controller controller)
        {
            settings = controller.settings;
        
            terrainLayer = 1 << LayerMask.NameToLayer(settings.Search("terrainLayer"));

            ToggleActive(true);
        }

        public virtual void FixedUpdateFeature(Controller controller)
        {
            if (!active) return;

            CheckTerrain(controller);
        }

        public abstract void CheckTerrain(Controller controller);

        public abstract Vector3 ProjectOnTerrain(Vector3 direction);

        public abstract Vector3 GetTerrainNormal();

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public static int CompareByOrder(TerrainModifier a, TerrainModifier b)
        {
            if (!a.OnTerrain && !b.OnTerrain)
            {
                return a.terrainOrder.CompareTo(b.terrainOrder);
            }

            if (a.OnTerrain && !b.OnTerrain)
            {
                return 1;
            }

            if (!a.OnTerrain && b.OnTerrain)
            {
                return -1;
            }

            return a.terrainOrder.CompareTo(b.terrainOrder);
        }
    }
}

