using System;
using UnityEngine;

public abstract class CombatEntity : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 1000f;
    private float currentHealth;
    private bool isDead = false;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;



    [Header("Combat")] [SerializeField] private float attackSpeed = .4f;
    [SerializeField] private float attackDamage = 100f;
    [SerializeField] protected float attackRange = 3f;
    
    private CombatEntity currentTarget;
    private float autoAttackTimer = 0f;
    protected bool isAutoAttacking = false;
    private float attacksPerSecond;

    
    
    // Spells
    protected bool isCasting = false;
    protected string currentCastName = "";
    protected float currentCastTime = 0f;
    protected float currentCastProgress = 0f;

    public bool IsCasting => isCasting;
    public string CurrentCastName => currentCastName;
    public float CastProgress => currentCastTime > 0 ? currentCastProgress / currentCastTime : 0f;
    
    // Events for UI and other updates later
    public event Action<float, float> OnHealthChanged; // current, max
    public event Action OnDeath;
    public event Action OnAutoAttack; // send signal after auto attacking

    public event Action<string, float> OnCastStart; // abilityName, castTime
    public event Action<string> OnCastComplete; // abilityName
    public event Action<string> OnCastInterrupted; // abilityName
    

    protected void Awake()
    {
        currentHealth = maxHealth;
        attacksPerSecond = 1 / attackSpeed;
    }

    protected void Update()
    {
        if (isDead) return;
        
        UpdateAutoAttack();
    }

    #region Auto Attacking

    
    private void UpdateAutoAttack()
    {
        
        // count up timer, when finished, deal damage to target and then reset the timer

        if (!isAutoAttacking || currentTarget == null) return;

        if (currentTarget.IsDead)
        {
            // stop auto attacking
            StopAutoAttacking();
            return;
        }
        
        // check range
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distance > attackRange)
        {
            //Debug.Log($"{name} is out of range! cannot attack!");
            return;
        }

        if (isCasting) return;

        autoAttackTimer -= Time.deltaTime;
        if (autoAttackTimer <= 0f)
        {
            // perform auto attack
            PerformAutoAttack();
            autoAttackTimer = 1f / attacksPerSecond; // reset timer
        }
    }

    private void PerformAutoAttack()
    {
        //Debug.Log($"{name} attacks {currentTarget.name} for {attackDamage} damage!");
        currentTarget.TakeDamage(attackDamage);
        OnAutoAttack?.Invoke();
    }
    public void StartAutoAttacking(CombatEntity target)
    {
        if (target == null || target.IsDead)
        {
            //Debug.Log($"{name} cannot attack null or dead target!");
            return;
        }

        if (isAutoAttacking && currentTarget == target)
            return;
        
        currentTarget = target;
        isAutoAttacking = true;
        autoAttackTimer = 0f; // First attack happens immediately
        
        //Debug.Log($"{name} starts attacking {target.name}");
    }
    public void StopAutoAttacking()
    {
        isAutoAttacking = false;
        currentTarget = null;
        //Debug.Log($"{name} stops attacking");
    }
    
    

    #endregion

    #region Health Changes
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        //Debug.Log($"{name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"{name} healed for {amount} HP. Health: {currentHealth}/{maxHealth}");
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        //Debug.Log($"{name} has died!");
        
        OnDeath?.Invoke();
    }

    #endregion

    #region Casting

    public void BeginCasting(string abilityName, float castTime)
    {
        if (!CanAct()) return;

        isCasting = true;
        currentCastName = abilityName;
        currentCastTime = castTime;
        currentCastProgress = 0;
        OnCastStart.Invoke(abilityName,castTime);
    }

    public void UpdateCastProgress(float progress)
    {
        currentCastProgress = progress;
    }

    public void CompleteCast()
    {
        isCasting = false;
        OnCastComplete.Invoke(currentCastName);
        currentCastName = "";

        autoAttackTimer = 1f / attacksPerSecond;
    }

    public void InterruptCast()
    {
        isCasting = false;
        OnCastInterrupted.Invoke(currentCastName);
        currentCastName = "";
    }

    #endregion
    public bool CanAct()
    {
        return !isDead && !isCasting;
    }

}
