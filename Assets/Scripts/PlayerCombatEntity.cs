using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerCombatEntity : CombatEntity
{
    [Header("Movement")] 
    [SerializeField] private InputAction xMoveInput;
    [SerializeField] private InputAction zMoveInput;
    [SerializeField] private float moveSpeed = 100f;

    private CharacterController controller;

    [Header("Fireball Spell")] 
    [SerializeField] private float fireballDamage = 300f;
    [SerializeField] private float fireballCastTime = 2.5f;
    [SerializeField] private float fireballCooldown = 5f;
    [SerializeField] private float fireballRange = 25f;
    private float fireballTimer = 0f;
    
    [Header("Heal Spell")] 
    [SerializeField] private float healAmount = 150f;
    [SerializeField] private float healCastTime = 2.5f;
    [SerializeField] private float healCooldown = 0f;
    [SerializeField] private float healRange = 25f;
    private float healTimer = 0f;

    [Header("Interrupt Ability")] 
    [SerializeField] private float interruptCooldown = 6f;
    [SerializeField] private float interruptRange = 4f;
    private float interruptTimer = 0f;
    
    [Header("Instant Spell")] 
    [SerializeField] private float instantDamage = 300f;
    [SerializeField] private float instantCastTime = 2.5f;
    [SerializeField] private float instantCooldown = 5f;
    [SerializeField] private float instantRange = 25f;
    private float instantTimer = 0f;

    public float FireballTimer => fireballTimer;
    public float FireballCooldown => fireballCooldown;
    public float HealTimer => healTimer;
    public float HealCooldown => healCooldown;
    public float InterruptTimer => interruptTimer ;
    public float InterruptCooldown => interruptCooldown ;
    public float InstantCastTimer => instantTimer;
    public float InstantCooldown => instantCooldown;
    

    private Vector3 lastPosition;
    private float movementThreshold = 0.01f;

    private BossCombatEntity currentTarget;

    [Header("Settings")] [SerializeField] private Animator playerAnim;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        base.Awake();
    }

    void Start()
    {
        xMoveInput.Enable();
        zMoveInput.Enable();
        lastPosition = transform.position;
        currentTarget = FindNearestEnemy();
        OnHealthChanged += HandleHitAnimations;
        OnAutoAttack += () => playerAnim.SetTrigger("Attack");

    }

    void Update()
    {
        if (IsDead) return;
        Vector3 targetPosition = currentTarget.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        HandleAnimations();
        CheckMovementInterrupt();

        Vector3 movement = new Vector3(xMoveInput.ReadValue<float>(), 0, zMoveInput.ReadValue<float>());
        movement.Normalize();
        controller.Move(movement * moveSpeed * Time.deltaTime);
        playerAnim.SetFloat("Move", movement.magnitude);

        if (fireballTimer > 0) 
            fireballTimer -= Time.deltaTime;
        
        if (healTimer > 0) 
            healTimer -= Time.deltaTime;

        if (interruptTimer > 0)
            interruptTimer -= Time.deltaTime;
        
        if (instantTimer > 0)
            instantTimer -= Time.deltaTime;

        if (!isAutoAttacking)
        {
            CombatEntity enemy = FindNearestEnemy();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= attackRange)
                {
                    StartAutoAttacking(enemy);
                }
            }
        }
        

        if (Keyboard.current.digit1Key.wasPressedThisFrame && CanAct())
            TryUseFireball();

        if (Keyboard.current.digit2Key.wasPressedThisFrame && CanAct())
            TryUseHeal();
        
        if(Keyboard.current.digit3Key.wasPressedThisFrame && CanAct())
            TryUseInterrupt();
        
        if(Keyboard.current.digit4Key.wasPressedThisFrame && CanAct())
            TryUseInstant();
                
        base.Update();
        
    }

    #region Interrupt Ability
    
    void TryUseInterrupt()
    {
        if (interruptTimer > 0)
        {
            Debug.Log("Interrupt is on cooldown");
            return;
        }
        
        BossCombatEntity target = FindNearestEnemy();
        if (target == null || target.IsDead)
        {
            Debug.Log("No valid Target!");
            return;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > interruptRange)
        {
            Debug.Log($"Out of range! {interruptRange} : {distance}");
            return;
        }
        
        lastPosition = transform.position;
        StartCoroutine(CastInterrupt(target));
    }

    IEnumerator CastInterrupt(BossCombatEntity target)
    {
        
        BeginCasting("Interrupt", 0);
        while (currentCastProgress < 0)
        {
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            if (target == null || target.IsDead)
            {
                Debug.Log("Target lost!");
                InterruptCast();
                yield break;
            }

            yield return null;
        }
        
        CompleteCast();
        Debug.Log("Used Interupt!!!");
        target.InterruptCurrentCast();
        playerAnim.SetTrigger("Special2");
        interruptTimer = interruptCooldown;
    }
    
    #endregion

    #region Fireball
    private void TryUseFireball()
    {
        if (fireballTimer > 0)
        {
            Debug.Log("Fireball is on cooldown!");
            return;
        }

        CombatEntity target = FindNearestEnemy();
        if (target == null || target.IsDead)
        {
            Debug.Log("No valid Target!");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > fireballRange)
        {
            Debug.Log("Out of range!");
            return;
        }

        lastPosition = transform.position;
        StartCoroutine(CastFireball(target));
    }

    private IEnumerator CastFireball(CombatEntity target)
    {
        BeginCasting("Fireball", fireballCastTime);
        Debug.Log("Casting Fireball....");
        
        

        while (currentCastProgress < fireballCastTime)
        {
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            if (target == null || target.IsDead)
            {
                Debug.Log("Target lost!");
                InterruptCast();
                yield break;
            }

            yield return null;
        }
        
        CompleteCast();
        Debug.Log("Fireball!!!");
        target.TakeDamage(fireballDamage);
        playerAnim.SetTrigger("Special1");
        fireballTimer = fireballCooldown;
    }
    #endregion
    
    #region Heal
    private void TryUseHeal()
    {
        if (healTimer > 0)
        {
            Debug.Log("Heal is on cooldown!");
            return;
        }

        CombatEntity target = this;
        if (target == null || target.IsDead)
        {
            Debug.Log("No valid Target!");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > healRange)
        {
            Debug.Log("Out of range!");
            return;
        }

        lastPosition = transform.position;
        StartCoroutine(CastHeal(target));
    }

    private IEnumerator CastHeal(CombatEntity target)
    {
        BeginCasting("Heal", healCastTime);
        Debug.Log("Casting Heal....");
        
        

        while (currentCastProgress < healCastTime)
        {
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            if (target == null || target.IsDead)
            {
                Debug.Log("Target lost!");
                InterruptCast();
                yield break;
            }

            yield return null;
        }
        
        CompleteCast();
        Debug.Log("Heal!!!");
        target.Heal(healAmount);
        healTimer = healCooldown;
    }
    #endregion
    
    #region Instant Attack
    private void TryUseInstant()
    {
        if (instantTimer > 0)
        {
            Debug.Log("Instant attack is on cooldown!");
            return;
        }

        CombatEntity target = FindNearestEnemy();
        if (target == null || target.IsDead)
        {
            Debug.Log("No valid Target!");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > fireballRange)
        {
            Debug.Log("Out of range!");
            return;
        }

        lastPosition = transform.position;
        StartCoroutine(CastInstant(target));
    }

    private IEnumerator CastInstant(CombatEntity target)
    {
        BeginCasting("Instant", instantCastTime);
        Debug.Log("Casting Instant attack....");
        
        

        while (currentCastProgress < instantCastTime)
        {
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            if (target == null || target.IsDead)
            {
                Debug.Log("Target lost!");
                InterruptCast();
                yield break;
            }

            yield return null;
        }
        
        CompleteCast();
        Debug.Log("instant attack!!!");
        target.TakeDamage(instantDamage);
        playerAnim.SetTrigger("Special1");
        instantTimer = instantCooldown;
    }
    #endregion


    void HandleAnimations()
    {
        playerAnim.SetBool("IsCasting", isCasting);
 
        
    }

    void HandleHitAnimations(float current, float max)
    {
        playerAnim.SetTrigger("Hit");
        if (current <= 0)
        {
            playerAnim.SetTrigger("Death");
        }
    }
    private void CheckMovementInterrupt()
    {
        if (!isCasting || CastProgress > 0.95f) return;
        
        
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        //Debug.Log($"DistanceMoved {distanceMoved}");

        if (distanceMoved > movementThreshold)
        {
            Debug.Log("Cast Interrupted by movement!");
            StopAllCoroutines();
            InterruptCast();
        }
    }
    
    private BossCombatEntity FindNearestEnemy()
    {
        // Simple version - finds any BossCombatEntity
        BossCombatEntity boss = FindObjectOfType<BossCombatEntity>();
        return boss;
    }
}
