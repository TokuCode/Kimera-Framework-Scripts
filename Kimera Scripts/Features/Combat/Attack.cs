using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class Attack :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate, IFeatureFixedUpdate //Other channels
    {
        //Configuration
        [Header("Settings")]
        public Controller controller;
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        //States
        [Header("States")]
        [SerializeField] private bool activeAttack;
        [SerializeField] private bool uniqueEffectsTriggered = false;
        [SerializeField] private Vector3 playerForward;
        [SerializeField] private string linkerGuid;
        public bool ActiveAttack { get => activeAttack; }
        [SerializeField] private AttackSwing actualAttack;
        //Properties
        //References
        [Header("References")]
        public AttackBox attackBox;
        //Componentes
        [Header("Components")]
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private Rigidbody rb;

        [Header("Debug")]
        public bool debug;

        private void Awake()
        {
            //Setup References
            attackBox = GetComponent<AttackBox>();

            //Setup Components
            rb = GetComponent<Rigidbody>();
        }

        public void SetupFeature(Controller controller)
        {
            this.controller = controller;
            settings = controller.settings;

            //Setup Properties

            ToggleActive(true);
        }

        public void UpdateFeature(Controller controller)
        {
            if(!active) return;
        
            InputEntity inputEntity = controller as InputEntity;
            if(inputEntity != null) playerForward = inputEntity.playerForward;
        }

        public void FixedUpdateFeature(Controller controller)
        {
            if (!active) return;

            KineticEntity kinetic = controller as KineticEntity;
            if(kinetic != null) kinetic.currentSpeed = playerRigidbody.velocity.magnitude;

            if (actualAttack != null && activeAttack) AttackEffects(actualAttack.settings, kinetic);
        }

        public void StartAttackBox(AttackSwing attack, float attackSpeedMultiplier)
        {
            if (!active || attack == null) return;

            actualAttack = attack;
            activeAttack = true;
            Vector3 attackDirection = transform.right * attack.movement.x + transform.up * attack.movement.y + transform.forward * attack.movement.z;
            attackBox.SetBox(attack.size, attack.offset, attackDirection, true);
            attackBox.SetAttack(attack.settings);   
            uniqueEffectsTriggered = false;

            CombatAnimatorLinker linker = controller.SearchFeature<CombatAnimatorLinker>();
            if (linker != null && attack.bodyPart != CombatAnimatorLinker.BodyParts.Unlink) linkerGuid = linker.CreateLink(transform, attack.bodyPart, (attack.end - attack.start) / attackSpeedMultiplier, attack.movementCurve, attack.movementLinker, attack.offset);

            if (attack.settings != null) attack.settings.AssemblySettings();
        }

        private void AttackEffects(Settings attackSettings, KineticEntity kinetic)
        {
            if (!active || attackSettings == null || playerRigidbody == null) return;

            //Contiuous Events During Swing
            AttackFollowForce(attackSettings, kinetic);
            AttackFollowVerticalForce(attackSettings, kinetic);

            if (uniqueEffectsTriggered) return;
            uniqueEffectsTriggered = true;

            //Unique Events
            DisplayAttackVFX();
            AttackImpulse(attackSettings);
            VerticalAttackImpulse(attackSettings);
        }

        private void DisplayAttackVFX(){
            string[] vfxNamesList = actualAttack.vfxNames.Split(",");

            if(vfxNamesList.Length <= 0) return;

            CombatAnimatorLinker linker = controller.SearchFeature<CombatAnimatorLinker>();

            Transform bodyPart;
            if(linker != null) bodyPart = linker.GetAnimationBodyPart(actualAttack.bodyPart);
            else bodyPart = controller.transform;

            if(ElementInstancer.instance!=null)
            {
                Transform attachTransform = bodyPart.transform;
                if (actualAttack.hitboxAttach) attachTransform = transform;

                for (int i = 0; i < vfxNamesList.Length; i++)
                {
                    if (vfxNamesList[i] == string.Empty) continue;
                    GameObject generatedObj;
                    if (vfxNamesList[i] != "")
                    {
                        generatedObj = ElementInstancer.instance.Generate(ElementInstancer.instance.GetObjectListValue(vfxNamesList[i]), attachTransform);
                        generatedObj.transform.rotation = attachTransform.rotation;
                        generatedObj.transform.position = attachTransform.position;
                    }
                    //ParticleSystem particle = VFXcontroller.instance?.InstanceVFX(vfxNamesList[i], bodyPart.position, Quaternion.identity);
                    //particle.transform.SetParent(bodyPart, true);

                    Combat combat = controller.SearchFeature<Combat>();

                    //Destroy(particle, (actualAttack.end - actualAttack.start) * (combat != null ? combat.attackSpeedMultiplier : 1f));
                }
            }
            
        }

        private void AttackFollowForce(Settings attackSettings, KineticEntity kinetic)
        {
            if (attackSettings == null || kinetic == null) return;

            float? attackFollowMove = attackSettings.Search("attackFrictionMove");

            if (attackFollowMove == null) return;

            if (attackFollowMove == 0) return;

            Vector3 playerSpeed = playerRigidbody.velocity;
            playerSpeed.y = 0;

            playerRigidbody.AddForce(-playerSpeed * (float)attackFollowMove, ForceMode.Acceleration);
        }

        private void AttackFollowVerticalForce(Settings attackSettings, KineticEntity kinetic)
        {
            if (attackSettings == null || kinetic == null) return;

            float? attackFollowMove = attackSettings.Search("attackFollowVerticalMove");

            if (attackFollowMove == null) return;

            if (attackFollowMove == 0) return;

            playerRigidbody.AddForce(controller.transform.up * (float)attackFollowMove, ForceMode.Acceleration);
        }

        private void AttackImpulse(Settings attackSettings)
        {
            if(attackSettings == null) return;

            float? attackImpulse = attackSettings.Search("attackImpulse");

            if(attackImpulse == null) return;

            if (attackImpulse == 0) return;

            playerRigidbody.AddForce(controller.transform.forward * (float)attackImpulse, ForceMode.VelocityChange);
        }

        private void VerticalAttackImpulse(Settings attackSettings)
        {
            if (attackSettings == null) return;

            float? attackImpulse = attackSettings.Search("attackImpulseVertical");

            if (attackImpulse == null) return;

            if (attackImpulse == 0) return;

            playerRigidbody.AddForce(controller.transform.up * (float)attackImpulse, ForceMode.VelocityChange);    
        }

        public void EndAttackBox()
        {
            actualAttack = null;
            activeAttack = false;
            attackBox.SetBox();
            attackBox.SetAttack();

            CombatAnimatorLinker linker = controller.SearchFeature<CombatAnimatorLinker>();
            if (linker != null) linker.DestroyLink(linkerGuid);
        }

        public bool GetActive()
        {
            return active;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;
        }

        //private void OnDrawGizmos()
        //{
        //    if (!debug || !Application.isPlaying) return;

        //    if (actualAttack == null || !activeAttack) return;

        //    Gizmos.matrix = transform.localToWorldMatrix;

        //    Gizmos.color = Color.green;
        //    Gizmos.DrawCube(Vector3.zero,actualAttack.size);
        //}
    }
}