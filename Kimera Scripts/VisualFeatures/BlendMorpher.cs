using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Android;

namespace Features
{
    public class BlendMorpher :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public SkinnedMeshRenderer cmp_smr;
        public BlendGroup[] activeObjects;
        public float gapActivationValue;
        public SkinMeshBlend[] meshRenderers;
        public SkinnedMeshRenderer[] emissionRenderers;
        [ColorUsage(true, true)]
        public UnityEngine.Color baseColor;
        [ColorUsage(true, true)]
        public UnityEngine.Color furyColor;
        public float emissionUnit;
        [Range(0.5f,10)]
        public float changeSpeed;
        [SerializeField]
        private float blendValue=0;
        
        private float time;
        public float maxValue = 100;
        public float blendUnit;
        
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            
            //Setup Properties

            ToggleActive(true);
            FurryEntity furryEntity = controller as FurryEntity;
            blendUnit = maxValue / settings.Search("furryMax");
            emissionUnit = 1 / settings.Search("furryMax");
            GroupEnabler(furryEntity.furryCount * blendUnit);
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        public void UpdateFeature(Controller controller)
        {
            //Debug.Log("Test");

            FurryEntity furryEntity = controller as FurryEntity;

            if (blendValue != furryEntity.furryCount) GroupEnabler(furryEntity.furryCount * blendUnit);

            blendValue = furryEntity.furryCount * blendUnit;

            if (meshRenderers.Length>0)
            {
                foreach (SkinMeshBlend mesh in meshRenderers)
                {
                    BlendChange(blendValue, mesh);
                }
            }
            if (emissionRenderers.Length > 0)
            {
                foreach (SkinnedMeshRenderer mesh in emissionRenderers)
                {
                    BlendEmission(furryEntity.furryCount * emissionUnit, mesh);
                    Debug.Log(furryEntity.furryCount * emissionUnit);
                }
            }
            /*if(cmp_smr.GetBlendShapeWeight(0)!=blendValue && time < changeSpeed)
            {                
                float currentWeight = cmp_smr.GetBlendShapeWeight(0);
                currentWeight = Mathf.Lerp(currentWeight, blendValue, time / changeSpeed);
                cmp_smr.SetBlendShapeWeight(0, currentWeight);

                time += Time.deltaTime;
            }*/


        }

        public void BlendChange(float value, SkinMeshBlend blend)
        {
            if (blend.skinMesh == null) return;

            float currentWeight = blend.skinMesh.GetBlendShapeWeight(0);
            if (currentWeight == value) return;

            if (blend.inverse == true) value = maxValue - value;
            currentWeight = Mathf.Lerp(currentWeight, value, changeSpeed);
            blend.skinMesh.SetBlendShapeWeight(0, currentWeight);
        }
        public void GroupEnabler(float value)
        {
            float baseValue = maxValue / activeObjects.Length;
            for (int i = 0; i < activeObjects.Length; i++)
            {
                bool toSet = false;
                float tmpVal = value;
                if (i == activeObjects.Length - 1) tmpVal--;
                else if(i==0) tmpVal++;
                if (tmpVal + gapActivationValue >= baseValue * i && tmpVal - gapActivationValue < baseValue * (i+1)) toSet = true;

                for (int j = 0; j < activeObjects[i].setOnElements.Length; j++)
                {
                    activeObjects[i].setOnElements[j].SetActive(toSet);
                }
            }
        }

        public void BlendEmission(float value, SkinnedMeshRenderer mesh)
        {
            if (mesh == null) return;
            Debug.Log(mesh.gameObject.name + " " + (value));
            //UnityEngine.Color previousColor = UnityEngine.Color.white; //mesh.sharedMaterial.GetColor("_EmissionColor");
            //UnityEngine.Color baseCol = mesh.material.GetColor("_EmissionColor");
            //float currentEmission = 0.2989f * baseCol.r + 0.5870f * baseCol.g + 0.1140f * baseCol.b;

            //UnityEngine.Color _emissionColor = mesh.material.GetColor("_EmissionColor");
            //var maxColorComponent = _emissionColor.maxColorComponent;
            //var scaleFactor = 191 / maxColorComponent;
            //float currentEmission = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);

            //value = value - 5;
            //Debug.Log(currentEmission + " | " + (value-5) + " | " + Mathf.Pow(2, currentEmission));
            //if (currentEmission == value ||  currentEmission < -5 ||  currentEmission > 5) return;
            //currentEmission = Mathf.Lerp(currentEmission, value-5, changeSpeed);
            //float lerpValue = Mathf.Lerp(lerpValue, value, changeSpeed);
            //Debug.Log(currentEmission + " | " + value + " | " + Mathf.Pow(2, currentEmission));
            //float intensityVal = Mathf.Pow(2, currentEmission);

            UnityEngine.Color newColor = new UnityEngine.Color(Mathf.Lerp(baseColor.r, furyColor.r, value), Mathf.Lerp(baseColor.g, furyColor.g, value), Mathf.Lerp(baseColor.b, furyColor.b, value), Mathf.Lerp(baseColor.a, furyColor.a, value));

            mesh.material.SetColor("_EmissionColor", newColor);
        }
    }

    [System.Serializable]
    public class BlendGroup
    {
        [Header("To Active Objects")]
        public GameObject[] setOnElements;
    }
    [System.Serializable]
    public class SkinMeshBlend
    {
        public bool inverse;
        public SkinnedMeshRenderer skinMesh;
    }

}