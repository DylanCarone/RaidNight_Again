using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerCombatEntity : CombatEntity
{
    [Header("Settings")] 
    [SerializeField] CharacterData characterData;
    [SerializeField] Transform spawnTempTransform;
    private Animator playerAnim;
    [SerializeField] private LayerMask losLayer;
    private Vector3 lastPosition;
    private float movementThreshold = 0.01f;
    private Vector3 movement;
    private PlayerRole playerRole;
    public PlayerRole Role => playerRole;

    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 4f;
    private CharacterController controller;
    private PlayerInput playerInput;
    public PlayerInput Inputs => playerInput;
    
    [Header("Global Cooldown")] 
    [SerializeField] private float globalCooldown = 2f;

    private float globalCooldownTimer = 0f;
    private float effectiveGlobalCooldown;
    public float GlobalCooldownTimer => globalCooldownTimer;
    public float GlobalCooldownDuration => effectiveGlobalCooldown;
    private bool IsOnGlobalCooldown => globalCooldownTimer > 0f;
    
    // resource
    private ResourceType resourceType;
    public ResourceType ResourceType => resourceType;
    private float maxResoruce;
    public float MaxResoruce => maxResoruce;
    private float currentResource;
    public float CurrentResource => currentResource;
    public Action<float,float> OnResourceChanged;
    private float regenPerSecond;
   
    // Spells
    private AbilityInstance squareSpell;
    private AbilityInstance triangleSpell;
    private AbilityInstance circleSpell;
    private AbilityInstance xSpell;
    public AbilityInstance SquareSpell => squareSpell;
    public AbilityInstance TriangleSpell => triangleSpell;
    public AbilityInstance CircleSpell => circleSpell;
    public AbilityInstance XSpell => xSpell;
    
    private List<AbilityInstance> spells = new List<AbilityInstance>();
    public List<AbilityInstance> Spells => spells;
    
    // Targeting
    private float targetRange;
    public float TargetRange => targetRange;
    private BossCombatEntity cachedBoss;
    
    bool isInitialized = false;

    [SerializeField] ParticleSystem castingParticles;
    [SerializeField] private GameObject attackParticles;

    private bool thresholdMet = false;

    #region Initialization

    /// <summary>
    /// Initializes player for multiplayer. Called by PlayerManager.
    /// If not called externally, Start() will initialize with default data.
    /// </summary>
    public void Initialize(CharacterData characterData, PlayerInput inputs, GameObject model)
    {
        this.characterData = characterData;
        playerRole = characterData.role;
        playerAnim = model.GetComponent<Animator>();
        playerInput = inputs;
        AddSpells();
        if(playerInput)
            playerInput.actions.Enable();
        maxHealth = characterData.maxHealth;
        attackSpeed = characterData.attackSpeed;
        attackDamage = characterData.attackDamage;
        attackRange = characterData.attackRange;
        resourceType =  characterData.resourceType;
        maxResoruce = characterData.resourceType.maxResource;
        regenPerSecond = characterData.resourceType.regenPerSecond;;
        
        if(characterData.resourceType.StartFull)
            currentResource = maxResoruce;
        if (characterData.resourceType.RestoreResourceOnHit)
            OnTakeDamage += RestoreResourceOnHit;

        OnResourceChanged += SetThreshhold;
        
        isInitialized = true;
        

    } 
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        //playerInput = GetComponent<PlayerInput>();
        //Debug.Log(playerInput.playerIndex);
        if(castingParticles) castingParticles.Stop();
        base.Awake();
    }


    void Start()
    {
        lastPosition = transform.position;
        effectiveGlobalCooldown = globalCooldown;
        //currentTarget = FindNearestEnemy();
        OnHealthChanged += HandleHitAnimations;
        OnAutoAttack += () => playerAnim.SetTrigger("Attack");

        if (!isInitialized && characterData != null)
        {
            Debug.Log("Not initialized yet, doing now");
            PlayerInput input = GetComponent<PlayerInput>();
            var model = Instantiate(characterData.playerModel, spawnTempTransform.position, spawnTempTransform.rotation, spawnTempTransform);
            Initialize(characterData, input, model);
        }
        cachedBoss = FindObjectOfType<BossCombatEntity>();
        currentHealth = maxHealth;
    }

    void AddSpells()
    {
        Debug.Log("Adding spells");
        squareSpell = new AbilityInstance(characterData.abilities[0]);
        triangleSpell = new AbilityInstance(characterData.abilities[1]);
        circleSpell = new AbilityInstance(characterData.abilities[2]);
        xSpell = new AbilityInstance(characterData.abilities[3]);
        spells.Add(squareSpell);
        spells.Add(circleSpell);
        spells.Add(triangleSpell);
        spells.Add(xSpell);
    }
    
    #endregion

    protected void Update()
    {
        if (!isInitialized) return;
        playerAnim.SetBool("Death", IsDead);
        if (IsDead) return;
        
        HandleAnimations();
        CheckMovementInterrupt();
        LookAtTarget();
        HandleInputs();
        HandleCooldowns();
        if(characterData.resourceType.AutoRegenerate)
            UpdateResourceRegen();
        

        if (!isAutoAttacking && Role != PlayerRole.Healer)
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

    protected override void PerformAutoAttack()
    {
        if(characterData.resourceType.RestoreOnAutoAttack)
            RestoreResource(regenPerSecond);
        
        base.PerformAutoAttack();
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
            TryCastSpell(squareSpell);
        
        if(playerInput.actions.FindAction("Ability 2").WasPressedThisFrame() && CanCastSpell())
            TryCastSpell(xSpell);
        
        if(playerInput.actions.FindAction("Ability 3").WasPressedThisFrame() && CanCastSpell())
            TryCastSpell(circleSpell);
        
        if (playerInput.actions.FindAction("Ability 4").WasPressedThisFrame() && CanCastSpell())
            TryCastSpell(triangleSpell);
    }


    #region Spells
    private void TriggerGlobalCooldown()
    { 
        globalCooldownTimer = characterData.resourceType.GetEffectiveGCD(globalCooldown, thresholdMet);
        effectiveGlobalCooldown = globalCooldownTimer;
    }

    public void TryCastSpell(AbilityInstance spell)
    {
        if (spell.GetCooldownRemaining() > 0) return;
        if (!HasResource(spell)) return;
        
        bool isResurrect = spell.ability is ResAbility;
        bool isThreshold = spell.ability is ThresholdAbility;
        CombatEntity target = currentTarget;
        

        if (isResurrect)
        {
            if (target == null ||!target.IsDead) return;
            target = currentTarget;
        }
        else if (isThreshold)
        {
            if (target == null || target.IsDead || !thresholdMet) return;
            target = currentTarget;
        }
        else if (spell.ability.targetType == AbilityTargetType.Self)
        {
            target = this;
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
        

        lastPosition = transform.position; // set position so it can cancel from walking
        castingParticles?.Play();
        StartCoroutine(CastSpell(target, spell));
        
    }

    private IEnumerator CastSpell(CombatEntity target, AbilityInstance spell)
    {
        
        float castTime = spell.GetModifiedCastTime();
        float resourceCost = spell.GetModifiedResourceCost();
        float resourceGained = spell.GetModifiedResourceGained();
        
        BeginCasting(spell.ability.abilityName, castTime);
        bool isResurrect = spell.ability is ResAbility;
        if(spell.ability.isOnGDC)
            TriggerGlobalCooldown();
        

        while (currentCastProgress < castTime)
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
        
        CompleteCast(); ;
        castingParticles?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        spell.ability.ExecuteAbility(this, target);

        foreach (var empowerment in spell.ActiveEmpowerments)
        {
            empowerment.OnSpellFired(this,target);
        }
        
        ConsumeResource(resourceCost);
        RestoreResource(resourceGained);
        spell.ConsumeEmpowerments();
        var attackVfx = Instantiate(attackParticles, target.transform.position + Vector3.up *0.5f, transform.rotation);
        Destroy(attackVfx, 2f);
        
        playerAnim.SetTrigger(spell.ability.animation.ToString());
        spell.StartCooldown();
    }
    public bool CanCastSpell() => CanAct() && !IsOnGlobalCooldown;
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


    public AbilityInstance GetSpellByAbility(Ability ability)
    {
        return spells.FirstOrDefault(s => s.ability == ability);
    }
    #endregion

    #region Resource

   
    bool HasResource(AbilityInstance spell)
    {
        return currentResource >= spell.GetModifiedResourceCost();
    }

    void ConsumeResource(float amount)
    {
        if(amount <= 0) return;
        currentResource = Mathf.Max(0, currentResource - amount);
        OnResourceChanged?.Invoke(currentResource, maxResoruce);
    }

    public void RestoreResource(float amount)
    {
        if(amount <= 0) return;
        currentResource = Mathf.Min(currentResource + amount, maxResoruce);
        OnResourceChanged?.Invoke(currentResource, maxResoruce);
    }

    void UpdateResourceRegen()
    {
        if (currentResource < maxResoruce)
        {
            RestoreResource(regenPerSecond * Time.deltaTime);
        }
        
    }

    void RestoreResourceOnHit()
    {
        RestoreResource(regenPerSecond);
    }

    void SetThreshhold(float current, float max)
    {
        thresholdMet = characterData.resourceType.IsThresholdActive(current, max);
    }


    #endregion
    
    #region Animations

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
        // Allow movement in last 5% of cast (feels more responsive)
        if (!isCasting || CastProgress > 0.95f) return;
        
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved > movementThreshold)
        {
            Debug.Log("Cast Interrupted by movement!");
            castingParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            StopAllCoroutines();
            InterruptCast();
        }
    }
    #endregion
    #region Death
    public void Resurrect(float healingAmount)
    {
        isDead = false;
        Heal(healingAmount);
    }

    protected override void Die()
    {
        StopAllCoroutines();
        playerAnim.Play("Death_A");
        base.Die();
    }
    
    

    #endregion
    #region Targeting
    
    public BossCombatEntity FindNearestEnemy()
    {
        // Simple version - finds any BossCombatEntity
        return cachedBoss;
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
    
    public void SetTarget(CombatEntity target) => currentTarget = target;
    #endregion


}


public enum PlayerRole
{
    Tank,
    Healer,
    Melee,
    Ranged
}