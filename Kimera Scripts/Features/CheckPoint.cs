using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

namespace Features
{
    public class CheckPoint :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
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
        [SerializeField] Image image;
        [SerializeField] float velocityFade;
        [SerializeField] SpawnPoint spwPoint;
        [SerializeField] Life life;
        [SerializeField] Ragdoll ragdoll;
        [SerializeField] Transform firstSpine;
        [SerializeField] Furry fury;
        [SerializeField] Controller contrl;
        Vector3 savePos;
        //References
        //Componentes

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            contrl = controller;
            //Setup Properties
            savePos = firstSpine.localPosition;
            life.OnDeath += () => RespawnPosition();

            fury = controller.SearchFeature<Furry>();

            ToggleActive(true);
        }

        public void RespawnPosition()
        {
            StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            image.gameObject.SetActive(true);
            Color currentColor = Color.black;
            currentColor.a = 0;
            
            while(currentColor.a <= 1)
            {
                currentColor.a += Time.deltaTime * velocityFade;
                image.color =currentColor;
                yield return null;
            }

            yield return new WaitForSeconds(2);
            firstSpine.transform.localPosition = savePos;
            ragdoll.RagdollSetActive(false);
            life.ResetHealth();
            if (fury != null) fury.furryCount = 0;
            if(RevertProgressActions.instance!=null) RevertProgressActions.instance.Reset();
            contrl.enabled = true;
            //contrl.ToggleActive(true);

            transform.position = new Vector3(spwPoint.x,spwPoint.y,spwPoint.z);

            while(currentColor.a >= 0)
            {
                currentColor.a -= Time.deltaTime * velocityFade;
                image.color =currentColor;
                yield return null;
            }
            yield return null;
            image.gameObject.SetActive(false);
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "checkpoint")
            {
                
                spwPoint.SettPoint(other.transform.position);
            }
        }
    }
}