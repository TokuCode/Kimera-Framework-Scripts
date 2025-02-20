using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class AttackBox : Hitbox
    {
        //States
        [Header("States")]
        public Settings attack;
        //Properties
        //Properties / Tag to include: specific tags
        //References
        //Componentes

        public override void SetupFeature(Controller controller)
        {
            base.SetupFeature(controller);

            string tag1 = settings.Search("attackHitboxTag1");
            string tag2 = settings.Search("attackHitboxTag2");
            string tag3 = settings.Search("attackHitboxTag3");
 
            if(tag1 != null) if(tag1 != string.Empty) tagsToInteract.Add(tag1);
            if(tag2 != null) if (tag2 != string.Empty) tagsToInteract.Add(tag2);
            if(tag3 != null) if (tag3 != string.Empty) tagsToInteract.Add(tag3);

        }

        protected override void InteractEntity(Controller interactor)
        {
            //Logica Link
            //Especificar el tipo de link que se va a crear
            Link link = new AttackLink(controller, interactor, attack);
        }

        protected override void InteractObject(GameObject gameObject)
        {
            //Logica otros sistemas
        }

        public void SetAttack(Settings attack = null)
        {
            this.attack = attack;
        }
    }
}
