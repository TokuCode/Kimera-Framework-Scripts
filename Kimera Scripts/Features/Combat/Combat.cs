using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Features
{
    public class Combat :  MonoBehaviour, IActivable, IFeatureSetup, IFeatureUpdate //Other channels
    {
        private const float DEFAULT_ATTACK_COOLDOWN = 1.25f;
        private const float DEFAULT_IN_BETWEEN_ATTACKS_TIME = .4f;

        //Configuration
        [Header("Settings")]
        public Settings settings;
        //Control
        [Header("Control")]
        [SerializeField] private bool active;
        public Controller mainController;
        //States
        [Header("States")]
        [SerializeField]
        private bool activeAttack;
        public bool ActiveAttack { get => activeAttack; }
        [SerializeField] private AttackPreset actualAttack;
        [SerializeField] private Queue<AttackPreset> attackQueue;
        [SerializeField] private ComboPreset actualCombo;
        //States / Time Management
        [SerializeField] private float attackTimer;
        public float AttackTimer { get => attackTimer; }
        [SerializeField] public float attackCooldownTimer;
        //Properties
        [Header("Properties")]
        public List<ComboPreset> defaultCombos;
        public int attack;
        public float attackCooldown;
        public float inBetweenAttacksTime;
        //References
        [Header("References")]
        public CombatAnimator combatAnimator;
        public List<Attack> possibleAttacks;
        public ISubcontroller movement;       
        public ISubcontroller movementAI;
        public FaceTarget faceTarget;
        public Friction friction;
        public float divideValue;
        [Header("Components")]
        Movement cmp_movement;
        Rotation cmp_rotate;
        Furry cmp_furry;
        EntityAnimator animator;

        [Header("Furry Stuff")]
        public float currentFurry = 0;
        public float attackSpeedModifier= 1.5f;
        public float attackSpeedMultiplier = 1f;
        public float baseSpeedModifier = 1f;

        private void Awake()
        {
            attackQueue = new Queue<AttackPreset>();

            //Setup References
            combatAnimator = GetComponent<CombatAnimator>();
            faceTarget = GetComponent<FaceTarget>();
            movement = GetComponent<Movement>() as ISubcontroller;
            cmp_movement = GetComponent<Movement>();
            cmp_rotate = GetComponent<Rotation>();
            movementAI = GetComponent<MovementModeSelector>() as ISubcontroller;
            friction = GetComponent<Friction>();
            cmp_furry = GetComponent<Furry>();
            animator = GetComponent<EntityAnimator>();
        }

        public void SetupFeature(Controller controller)
        {
            settings = controller.settings;
            mainController = controller;
            //Setup Properties
            attack = settings.Search("attack");

            defaultCombos = new List<ComboPreset>();

            ComboPreset combo1 = settings.Search("defaultCombo1") as ComboPreset;
            ComboPreset combo2 = settings.Search("defaultCombo2") as ComboPreset;
            ComboPreset combo3 = settings.Search("defaultCombo3") as ComboPreset;
            ComboPreset combo4 = settings.Search("defaultCombo4") as ComboPreset;
            ComboPreset combo5 = settings.Search("defaultCombo5") as ComboPreset;
            ComboPreset combo6 = settings.Search("defaultCombo6") as ComboPreset;

            if (combo1 != null) defaultCombos.Add(combo1);
            if (combo2 != null) defaultCombos.Add(combo2);
            if (combo3 != null) defaultCombos.Add(combo3);
            if (combo4 != null) defaultCombos.Add(combo4);
            if (combo5 != null) defaultCombos.Add(combo5);
            if (combo6 != null) defaultCombos.Add(combo6);

            if (settings.Search("movementDivideValue") != null) divideValue = settings.Search("movementDivideValue");
            float? tempAttackCooldown = settings.Search("attackCooldown");
            if (tempAttackCooldown.HasValue) attackCooldown = tempAttackCooldown.Value;
            else attackCooldown = DEFAULT_ATTACK_COOLDOWN;

            float? tempInBetweenAttacksTime = settings.Search("inBetweenAttacksTime");
            if (tempInBetweenAttacksTime.HasValue) inBetweenAttacksTime = tempInBetweenAttacksTime.Value;
            else inBetweenAttacksTime = DEFAULT_IN_BETWEEN_ATTACKS_TIME;

            ToggleActive(true);
        }

        public void StartCombo(List<string> conditions)
        {
            if (!active) return;

            if (attackQueue.Count <= 0 && !activeAttack && attackTimer <= 0f && actualCombo != null)
            {
                actualCombo = null;
                attackCooldownTimer = attackCooldown;
            }

            if (actualCombo == null && attackCooldownTimer <= 0)
            {
                for (int i = 0; i < defaultCombos.Count; i++)
                {
                    var combo = defaultCombos[i];

                    if (conditions.Contains(combo.condition) && combo.attackChain.Length > 0)
                    {
                        actualCombo = combo;

                        for (int j = 0; j < combo.attackChain.Length; j++)
                        {
                            attackQueue.Enqueue(combo.attackChain[j]);
                        }
                        break;
                    }
                }
            } else if (actualCombo != null) {
                if (actualCombo.interruptions.Length > 0)
                {
                    for (int i = 0; i < actualCombo.interruptions.Length; i++)
                    {
                        var interruption = actualCombo.interruptions[i];

                        if (conditions.Contains(interruption.condition) && interruption.nextCombo != null)
                        {
                            actualCombo = interruption.nextCombo;

                            if (actualCombo.attackChain.Length == 0 || !conditions.Contains(actualCombo.condition)) continue;

                            attackQueue.Clear();

                            for (int j = 0; j < actualCombo.attackChain.Length; j++)
                            {
                                attackQueue.Enqueue(actualCombo.attackChain[j]);
                            }
                            break;
                        }
                    }
                }
            } 
        }

        public void CancelCombo(){
                StopAttack();
                attackCooldownTimer = 0;
                attackTimer = 0;
                combatAnimator.CancelCondition("stop");
            Debug.Log("Cancel");
        }

        public void UpdateFeature(Controller controller)
        {
            if (attackTimer > 0) attackTimer -= Time.deltaTime;
            if(attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;

            if (!active) return;

            if(combatAnimator.GetActiveConditions().Contains("stop")) CancelCombo();

            //StartCombo(combatAnimator.GetActiveConditions());
            StartCombo(combatAnimator.GetActiveLastCondition());

            CombatEntity combat = controller as CombatEntity;
            if (combat != null) combat.attack = attack;

            //SR: Probando Linea
            if (attackTimer < 0 && activeAttack) possibleAttacks.ForEach(attack => attack.EndAttackBox());

            activeAttack = false;
            possibleAttacks.ForEach(attack =>
            {
                if (attack.ActiveAttack) activeAttack = true;
            });
            // SR: * MULTIPLICADOR PERO CHEQUEAR
            if (!activeAttack && attackTimer <= inBetweenAttacksTime && attackCooldownTimer <= 0f && attackQueue.Count > 0 && combatAnimator.CheckCondition(actualCombo.condition))
            {
                SetupAttack(attackQueue.Dequeue(), controller);
                combat.comboCount++;
            }

            else if (attackTimer <= inBetweenAttacksTime && !activeAttack && actualAttack != null)
            {
                StopAttack();
                combat.comboCount = 0;
            }

            FurryEntity furry = controller as FurryEntity;
        }


        public void SetupAttack(AttackPreset attack, Controller controller)
        {
            if (movement != null) movement.ToggleActiveSubcontroller(false);

            if (attack.unallowPlayerControlOverride)
            {
                if (cmp_rotate != null) cmp_rotate.ToggleActive(false);
                if (cmp_movement != null) cmp_movement.ToggleActive(false);
                if (friction != null) friction.ToggleActive(true);
                if (faceTarget != null) faceTarget.ToggleActive(false);

            }
            else
            {
                if (cmp_movement != null) cmp_movement.DivideSpeed(divideValue);
                if (cmp_movement != null) cmp_movement.ToggleActive(true);
                if (friction != null) friction.ToggleActive(false);
                if (cmp_rotate != null) cmp_rotate.ToggleActive(true);
                if (faceTarget != null) faceTarget.ToggleActive(true);
            }
            
            //if (friction != null) friction.ToggleActive(false);
            if (movementAI != null) movementAI.ToggleActiveSubcontroller(false);
            //if (faceTarget != null) faceTarget.ToggleActive(true);

            if(cmp_furry != null)
            currentFurry = cmp_furry.furryCount/cmp_furry.furryMax;

            attackSpeedMultiplier = baseSpeedModifier + (currentFurry * attackSpeedModifier);

            actualAttack = attack;
            attackTimer = attack.animationClipHuman.length / attackSpeedMultiplier + inBetweenAttacksTime;
            

            combatAnimator.SetVariableInputPermanenceTime(attack.animationClipHuman.length / attackSpeedMultiplier);

            if (animator == null) return; 
            
            Animator cmp_animator = animator.cmp_animator;
            AnimatorOverrideController animatorOverride = new AnimatorOverrideController(cmp_animator.runtimeAnimatorController);
            animatorOverride["Humano_Strike1"] = attack.animationClipHuman;
            animatorOverride["Furro_Strike1"] = attack.animationClipBeast;
            cmp_animator.runtimeAnimatorController = animatorOverride;

            animator.FeatureAction(controller, new Setting("triggerName", "Attack", Setting.ValueType.String));
        }

        public void StartAttack(int i)
        {
            if (!active || i < 0 || i >= possibleAttacks.Count) return;
            if (actualAttack == null) return;
            possibleAttacks[i].StartAttackBox(actualAttack.swings[i], attackSpeedMultiplier);

            float? invincibleTime = actualAttack.swings[i].settings.Search("invincibleTime");
            if (invincibleTime.HasValue && mainController.SearchFeature<Life>()) mainController.SearchFeature<Life>().CallInmunity(invincibleTime.Value);

            Debug.Log("Started Attack " + i + "  |  " + actualAttack.name + " - " + actualAttack.swings[i].swingName);
            /*if (AudioManager.instance)
            {
                AudioManager.instance.PlaySound("Golpe");
            }*/
        }

        public void EndAttack(int i)
        {
            if (i < 0 || i >= possibleAttacks.Count) return;

            if (actualAttack == null) return;

            possibleAttacks[i].EndAttackBox();
        }

        public void StopAttack()
        {
            
            CombatEntity combatEnt = mainController as CombatEntity;

            if (combatEnt != null && !combatEnt.block)
            {
                Debug.Log("block not detected");
                if (movement != null) movement.ToggleActiveSubcontroller(true);   
            }

            if (cmp_rotate != null) cmp_rotate.ToggleActive(true);
            if (cmp_movement != null) cmp_movement.DivideSpeed(1);
            if (cmp_movement != null) cmp_movement.ToggleActive(true);
            if (friction != null) friction.ToggleActive(true);

            if (movementAI != null) movementAI.ToggleActiveSubcontroller(true);
            if (faceTarget != null) faceTarget.ToggleActive(false);

            for (int i = 0; i < possibleAttacks.Count; i++)
            {
                EndAttack(i);
            }

            actualAttack = null;
            actualCombo = null;
            attackCooldownTimer = attackCooldown;
            activeAttack = false;
            attackQueue.Clear();
            combatAnimator.SetVariableInputPermanenceTime(0f);
        }

        public void PriorityBasedCancelAttack(int incomingAttacImpact)
        {
            if(actualAttack == null) return;

            int attackToughness = actualAttack.attackToughness;

            if (attackToughness < incomingAttacImpact) StopAttack();
        }

        public bool GetActive()
        {
            return active;
        }

        public void SetAttack(int attack)
        {
            this.attack = attack;
        }

        public void ToggleActive(bool active)
        {
            this.active = active;

            if (active) return;

            StopAttack();
        }
    }
}