using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Features
{
    public class Interactable :  MonoBehaviour, IActivable, IFeatureSetup //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        //Properties
        public UnityEvent[] eventList;
        public bool onFirstTriger;
        public bool loopable;
        private int index = 0;
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

        public void ExcecuteAction()
        {
            if (eventList.Length > 0)
            {
                eventList[index].Invoke();

                if (index < eventList.Length - 1)
                {
                    index++;
                }
                else if (loopable == true)
                {
                    index = 0;
                }
            }
        }
    }
}
