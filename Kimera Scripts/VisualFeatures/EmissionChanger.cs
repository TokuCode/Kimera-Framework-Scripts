using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class EmissionChanger :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public Renderer[] materials;
        public float lerpDuration;
        private float emissionIntensity;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;

            //Setup Properties

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
        public void ChangeAll(float value)
        {
            for(int i=0;i<materials.Length;i++)
            {
                //StartCoroutine(EmissionChange(i, value));
            }
        }
        public IEnumerator EmissionChange(int materialIndex, int changeType)
        {
            float t = 0;

            if (changeType == 0)
            {
                while (true)
                {
                    yield return null;
                    emissionIntensity = Mathf.Lerp(5, -10, t / lerpDuration);

                    float adjustedIntensity = emissionIntensity - (0.4169f);
                    Color actualCol = materials[materialIndex].material.GetColor("_EmissionColor");
                    materials[materialIndex].material.SetColor("_EmissionColor", new Color(0, 0.75f, 0.01f) * Mathf.Pow(2, adjustedIntensity));

                    t += Time.deltaTime;

                    if (t > lerpDuration)
                    {
                        break;
                    }
                }
            }
            else if (changeType == 1)
            {
                while (true)
                {
                    yield return null;
                    emissionIntensity = Mathf.Lerp(-10, 5, t / lerpDuration);

                    float adjustedIntensity = emissionIntensity - (0.4169f);

                    materials[materialIndex].material.SetColor("_EmissionColor", new Color(0, 0.75f, 0.01f) * Mathf.Pow(2, adjustedIntensity));

                    t += Time.deltaTime;

                    if (t > lerpDuration)
                    {
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Esta corrutina solo acepta valores de 0 y 1, 0 para apagar el material y 1 para encederlo");
                StopCoroutine("emissionChange");
            }
        }
    }
}

