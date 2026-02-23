using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerCombatEntity : CombatEntity
{
    [SerializeField] private PlayerRole playerRole;
    public PlayerRole Role => playerRole;
    public int controllerIndex = 0;
    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 100f;

    private CharacterController controller;
    private PlayerInput playerInput;
    public PlayerInput Inputs => playerInput;
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;
    
    [Header("Global Cooldown")] 
    [SerializeField] private float globalCooldown = 2f;

    private float globalCooldownTimer = 0f;
    public float GlobalCooldownTimer => globalCooldownTimer;
    public float GlobalCooldownDuration => globalCooldown;
    public bool IsOnGlobalCooldown => globalCooldownTimer > 0f;
    
    
    [Header("Spell Book")] 
    [SerializeField] private AbilityInstance squareSpell;
    [SerializeField] private AbilityInstance triangleSpell;
    [SerializeField] private AbilityInstance circleSpell;
    [SerializeField] private AbilityInstance xSpell;
    private List<AbilityInstance> spells = new  List<AbilityInstance>();
    public AbilityInstance SquareSpell => squareSpell;
    public AbilityInstance TriangleSpell => triangleSpell;
    public AbilityInstance CircleSpell => circleSpell;
    public AbilityInstance XSpell => xSpell;

    private float targetRange;
    public float TargetRange => targetRange;
    

    [Header("Settings")] 
    [SerializeField] private Animator playerAnim;
    [SerializeField] private LayerMask losLayer;
    private Vector3 lastPosition;
    private float movementThreshold = 0.01f;
    private Vector3 movement;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        if(playerInput)
            playerInput.actions.Enable();
        
        //Debug.Log(playerInput.playerIndex);

        base.Awake();
    }

    public void SetPlayerController(PlayerInput input)
    {
        playerInput = input;
        isOccupied = true;
        playerInput.actions.Enable();
    }

    public void ReleaseController()
    {
        if(playerInput != null) Destroy(playerInput.gameObject);
        isOccupied = false;
        playerInput.actions.Disable();
    }
    void Start()
    {
        lastPosition = transform.position;
        //currentTarget = FindNearestEnemy();
        OnHealthChanged += HandleHitAnimations;
        OnAutoAttack += () => playerAnim.SetTrigger("Attack");
        AddSpells();
    }

    void AddSpells()
    {
        spells.Add(squareSpell);
        spells.Add(triangleSpell);
        spells.Add(circleSpell);
        spells.Add(xSpell);
    }

    void SetControllerIndex()
    {
        if (!playerInput.user.valid)
        {
            // This force-creates the user link if it's missing in the scene
            playerInput.SwitchCurrentControlScheme(Gamepad.all[controllerIndex]);
        }
        if (Gamepad.all.Count > controllerIndex)
        {
            // This is the "Magic Line"
            // It unpairs all devices and pairs ONLY the specific gamepad index
            playerInput.user.UnpairDevices();
            InputUser.PerformPairingWithDevice(Gamepad.all[controllerIndex], playerInput.user);
            
            //Debug.Log($"Player {gameObject.name} paired to {Gamepad.all[controllerIndex].name}");
        }
        else
        {
            Debug.LogWarning($"Controller index {controllerIndex} not found! Use Keyboard?");
        }
    }

    protected void Update()
    {
        playerAnim.SetBool("Death", IsDead);
        if (IsDead) return;
        
        HandleAnimations();
        CheckMovementInterrupt();
        LookAtTarget();
        HandleInputs();
        HandleCooldowns();
        

        if (!isAutoAttacking && playerRole != PlayerRole.Healer)
        {
            CombatEntity enemy = FindNearestEnemy();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                targetRange = distance;
                if (distance <= attackRange)
                {
                    StartAutoAttacking(enemy);
                }
            }
        }

        if (currentTarget != null)
        {
            float targetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
            targetRange = targetDistance;
        }

        base.Update();
        
    }

    private void LookAtTarget()
    {
        if (currentTarget == null || currentTarget == this)
        {
            // 1. Get the camera's forward and right vectors
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            // 2. Flatten them so we don't look "up" into the sky or "down" into the floor
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // 3. Create a movement direction relative to the camera
            Vector3 relativeMovement = (camForward * movement.z) + (camRight * movement.x);

            if (relativeMovement.sqrMagnitude > 0.01f)
            {
                Quaternion newRotation = Quaternion.LookRotation(relativeMovement);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f);
            }

            return;
        }
        
        
        if (currentTarget != null) 
        {
            Vector3 targetPosition = currentTarget.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
            return;
        }
        
        
        
    }

    private void HandleCooldowns()
    {
        if (globalCooldownTimer > 0)
            globalCooldownTimer -= Time.deltaTime;

        foreach (var spell in spells)
        {
            if(!spell.IsReady())
                spell.UpdateCooldown(Time.deltaTime);
        }

    }

    private void HandleInputs()
    {
        if(playerInput == null) return;
        Vector2 input = playerInput.actions.FindAction("Movement").ReadValue<Vector2>();
        movement = new Vector3(input.x, 0, input.y);
        movement.Normalize();
        controller.Move(movement * (moveSpeed * Time.deltaTime));
        playerAnim.SetFloat("Move", movement.magnitude);
        
        if (playerInput.actions.FindAction("Ability 1").WasPressedThisFrame() && CanCastSpell())
            TryUseSpell(squareSpell);
        
        if(playerInput.actions.FindAction("Ability 2").WasPressedThisFrame() && CanCastSpell())
            TryUseSpell(xSpell);
            
        
        if(playerInput.actions.FindAction("Ability 3").WasPressedThisFrame() && CanAct())
            TryUseSpell(circleSpell);
        
        if (playerInput.actions.FindAction("Ability 4").WasPressedThisFrame() && CanCastSpell())
            TryUseSpell(triangleSpell);
    }

    private bool CanCastSpell()
    {
        return CanAct() && !IsOnGlobalCooldown;
    }

    private void TriggerGlobalCooldown()
    {
        globalCooldownTimer = globalCooldown;
    }

    private void TryUseSpell(AbilityInstance spell)
    {
        if (spell.GetCooldownRemaining() > 0) return;
        
        bool isResurrect = spell.ability is ResAbility;
        CombatEntity target = currentTarget;

        if (isResurrect)
        {
            if (target == null ||!target.IsDead) return;
            target = currentTarget;
        }
        else if (spell.ability.damage > 0 && currentTarget.CurrentType == EntityType.Boss)  // damage targets boss
            target = FindNearestEnemy();
        else if (spell.ability.healing > 0 && currentTarget.CurrentType == EntityType.Player) // heals are self for now
            target = currentTarget;
        
        if ((target == null || (target.IsDead && !isResurrect))) // no valid target
            return;

        if (spell.ability.damage > 0 && currentTarget.CurrentType == EntityType.Player)
            return;
        

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > spell.ability.range) // out of range
            return;
        
        Vector3 directionToBoss = (target.transform.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToBoss, out hit, distance, losLayer)) // no LOS of target
            return;
        

        Debug.Log($"Using {spell.ability.abilityName} on {target.name}");
        lastPosition = transform.position; // set position so it can cancel from walking
        StartCoroutine(CastSpell(target, spell));
    }

    private IEnumerator CastSpell(CombatEntity target, AbilityInstance spell)
    {
        BeginCasting(spell.ability.abilityName, spell.ability.castTime);
        bool isResurrect = spell.ability is ResAbility;
        if(spell.ability.isOnGDC)
            TriggerGlobalCooldown();
        

        while (currentCastProgress < spell.ability.castTime)
        {
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            
            
            if (target == null || (target.IsDead && !isResurrect))
            {
                Debug.Log("Target lost!");
                InterruptCast();
                yield break;
            }

            yield return null;
        }
        
        CompleteCast();
        spell.ability.ExecuteAbility(this, target);
        playerAnim.SetTrigger(spell.ability.animation.ToString());
        spell.StartCooldown();
    }




    void HandleAnimations()
    {
        playerAnim.SetBool("IsCasting", isCasting);
    }

    void HandleHitAnimations(float current, float max)
    {
        playerAnim.SetTrigger("Hit");
    }
    private void CheckMovementInterrupt()
    {
        if (!isCasting || CastProgress > 0.95f) return;
        
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved > movementThreshold)
        {
            Debug.Log("Cast Interrupted by movement!");
            StopAllCoroutines();
            InterruptCast();
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        playerAnim.Play("Death_A");
        base.Die();
    }

    public BossCombatEntity FindNearestEnemy()
    {
        // Simple version - finds any BossCombatEntity
        BossCombatEntity boss = FindObjectOfType<BossCombatEntity>();
        return boss;
    }
    
    public void SetTarget(CombatEntity target) => currentTarget = target;

    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        TakeDamage(500);
    }

    public void Resurrect(float healingAmount)
    {
        isDead = false;
        Heal(healingAmount);
    }
}


public enum PlayerRole
{
    Tank,
    Healer,
    Melee,
    Ranged
}