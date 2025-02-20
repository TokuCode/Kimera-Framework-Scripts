using Features;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Controller, InputEntity, KineticEntity, TerrainEntity, SpecialTerrainEntity, CombatEntity, FurryEntity, LivingEntity, StunEntity, FollowEntity
{
    //States
    //Input
    public PlayerInput input;
    public Vector2 inputDirection { get; set; }
    public Vector3 playerForward { get; set; }
    public Camera playerCamera { get; set; }
    //Kinetic
    public Vector3 speed { get; set; }
    public float currentSpeed { get; set; }
    public float maxSpeed { get; set; }
    //Terrain
    public bool onGround { get; set; }
    public bool onSlope { get; set; }
    //SpecialTerrain
    public bool onLadder { get; set; }
    //Combat
    public bool block { get; set; }
    public bool parry { get; set; }
    public int attack { get; set; }
    public int comboCount { get; set; }
    //Furry
    public float furryCount { get; set; }
    public float maxFurryCount { get; set; }
    public int furryCombo {  get; set; }
    //Living
    public int currentHealth { get; set; }
    public int maxHealth { get; set; }
    // Stun
    public bool isStunned { get; set; }

    // Dash Distance
    public float maxDashDistance { get; set; }
    bool dashing;
    float currentFury;
    float audio_currentSpeed = 0;

    //Follow Entity
    public GameObject target {  get; set; }

    [Header("Properties")]
    public bool triggerDownAttack;

    [Header("Components")]
    public Transform dashPoint;
    public Camera_System cameraSys;
    public EntityAnimator animator;

    private void OnEnable()
    {
        GameManager.StateChanged += CheckInputState;
        SearchFeature<Life>().OnDeath += OnDeath;
        GameManager.manager.CheckState();
        
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= CheckInputState;
        SearchFeature<Life>().OnDeath -= OnDeath;
        GameManager.manager.CheckState();

    }

    public override void Setup()
    {
        playerCamera = Camera.main;
        triggerDownAttack = false;
        base.Setup();
    }

    private void Start()
    {
        AudioManager.instance.PlaySound("IdleHumano");
        AudioManager.instance.PlaySound("IdleBestia");
        animator = gameObject.GetComponent<EntityAnimator>();
    }

    protected override void Update()
    {
        if (!active) return;

        if (onGround == true) triggerDownAttack = false;

        if(currentFury != SearchFeature<Furry>().furryCount || Mathf.Abs(audio_currentSpeed - SearchFeature<Movement>().cmp_rigidbody.velocity.magnitude) > 0.1f)
        {
            audio_currentSpeed = SearchFeature<Movement>().cmp_rigidbody.velocity.magnitude;
            currentFury = SearchFeature<Furry>().furryCount;
            UpdateIdle();
        }

        base.Update();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();
        inputDirection = new Vector2(direction.y, direction.x);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (block || dashing) return;
        if (context.performed)
        {
            CallFeature<Features.Jump>();
            SearchFeature<Life>().Inmunity();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (block || SearchFeature<Dash>().dashCooldownTimer > 0 || SearchFeature<Dash>().IsDashing || SearchFeature<Dash>().IsCharging)
        {
            if (context.performed) AudioManager.instance.PlaySound("ViperCancel");
            return;
        }
        if (context.performed)
        {
            if (onGround == false || SearchFeature<Features.Jump>().hasJumped) return;
            // SearchFeature<Rotation>().RotateTo(dashPoint.Position);
            //CallFeature<Features.Dash>(new Setting("dashPoint", dashPoint.Position, Setting.ValueType.Vector3));
            //CallFeature<Ragdoll>(new Setting("ragdollActivation", true, Setting.ValueType.Bool));
            dashing = true;
            SearchFeature<TimeFocusMode>().EnableFocus();
            cameraSys.lockSys.AimVisual(true);
        }
        if(context.canceled && dashing)
        {
            dashing = false;
            cameraSys.lockSys.AimVisual(false);
            SearchFeature<TimeFocusMode>().DisableFocus();
            if (maxDashDistance <= 0) maxDashDistance = 15;
            if (cameraSys.GetCameraLookat(maxDashDistance).HasValue)
            {
                //if (animator != null) animator.cmp_animator.SetTrigger("DashExecute");
                dashPoint.position = cameraSys.GetCameraLookat(maxDashDistance).Value;
                CallFeature<Features.Dash>(new Setting("dashPoint", dashPoint.position, Setting.ValueType.Vector3));
                
            }
            
        }
    }

    public void OnDeath()
    {
        CallFeature<Ragdoll>(new Setting("ragdollActivation", true, Setting.ValueType.Bool));
        SoundLibrary soundLibrary = GetComponent<SoundLibrary>();
        //ToggleActive(false);
        this.enabled = false;

        if (soundLibrary != null)
        {
            soundLibrary.CallAudioManager("Muerte");
        }
        //ToggleActive(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (block) return;
        if (context.performed)
        {
            if (!onGround && !triggerDownAttack)
            {
                CallFeature<CombatAnimator>(new Setting("combatCondition", "attack-down", Setting.ValueType.String));
                CallFeature<AttackOnLand>(new Setting("On Land Attack", "attack-down-impact", Setting.ValueType.String));
                AudioManager.instance.PlaySound("GolpeAereo");
                triggerDownAttack = true;
                return;
            }

            CallFeature<CombatAnimator>(new Setting("combatCondition", "attack-normal", Setting.ValueType.String));
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (block) return;
        if (context.performed)
        {
            if(SearchFeature<CooldownAction>())
            {
                if(SearchFeature<CooldownAction>().GetCooldowntListValue("SpecialAtk")>=0)
                {
                    SearchFeature<CooldownAction>().VerifyToActivate(SearchFeature<CooldownAction>().GetCooldowntListValue("SpecialAtk"), () =>
                    {
                        CallFeature<CombatAnimator>(new Setting("combatCondition", "attack-special", Setting.ValueType.String));
                    });
                    return;
                }
            }

            CallFeature<CombatAnimator>(new Setting("combatCondition", "attack-special", Setting.ValueType.String));
        }

    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed) CallFeature<Block>(new Setting("toggleBlock", true, Setting.ValueType.Bool));
        if (context.canceled) CallFeature<Block>(new Setting("toggleBlock", false, Setting.ValueType.Bool));
    }
    public void CheckInputState()
    {
        if (GameManager.gameState == GameManager.GameState.Gameplay)
        {
            input.enabled = true;
        }
        else
        {
            input.enabled = false;
        }
    }

    void UpdateIdle()
    {
        float velocity = SearchFeature<Movement>().cmp_rigidbody.velocity.magnitude;

        float volumeRatio = Mathf.InverseLerp(0, SearchFeature<Furry>().furryMax, SearchFeature<Furry>().furryCount);

        float volumeValue = Mathf.Lerp(0, 0.35f, volumeRatio);

        float inverseVolumeValue = Mathf.Lerp(0.35f, 0, volumeRatio);

        if (velocity < 0.1f)
        {
            AudioManager.instance.ChangeVolume("IdleHumano", inverseVolumeValue, 0.2f);
            AudioManager.instance.ChangeVolume("IdleBestia", volumeValue, 0.2f);
        }
        else
        {
            AudioManager.instance.ChangeVolume("IdleHumano", 0, 0.2f);
            AudioManager.instance.ChangeVolume("IdleBestia", 0, 0.2f);
        }
    }
}
