using System;
using UnityEngine;

public abstract class CombatEntity : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private EntityType type;
    public EntityType CurrentType => type;
    
    [Header("Health")]
    [SerializeField] protected float maxHealth = 1000f;
    protected float currentHealth;
    protected bool isDead = false;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;



    [Header("Combat")] [SerializeField] protected float attackSpeed = 2.5f;
    [SerializeField] protected float attackDamage = 100f;
    [SerializeField] protected float attackRange = 3f;
    
    protected CombatEntity currentTarget;
    private float autoAttackTimer = 0f;
    protected bool isAutoAttacking = false;
    private float attacksPerSecond;
    
    // Spells
    protected bool isCasting = false;
    private string currentCastName = "";
    private float currentCastTime = 0f;
    protected float currentCastProgress = 0f;

    public bool IsCasting => isCasting;
    public float CastProgress => currentCastTime > 0 ? currentCastProgress / currentCastTime : 0f;
    
    // Events for UI and other updates later
    public event Action<float, float> OnHealthChanged; // current, max
    public event Action OnTakeDamage;
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
    
    protected virtual void UpdateAutoAttack()
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
        if (distance > attackRange) // out of range
            return;
        

        if (isCasting) return;

        autoAttackTimer -= Time.deltaTime;
        if (autoAttackTimer <= 0f)
        {
            // perform auto attack
            PerformAutoAttack();
            autoAttackTimer = attackSpeed; // reset timer
        }
    }

    protected virtual void PerformAutoAttack()
    {
        //Debug.Log($"{name} attacks {currentTarget.name} for {attackDamage} damage!");
        if (currentTarget.CurrentType == type) return; // dont attack if is friendly
        currentTarget.TakeDamage(attackDamage);
        OnAutoAttack?.Invoke();
    }
    public void StartAutoAttacking(CombatEntity target)
    {
        if (target == null || target.IsDead)
            return;
        

        if (isAutoAttacking && currentTarget == target)
            return;
        
        currentTarget = target;
        isAutoAttacking = true;
        autoAttackTimer = 0f; // First attack happens immediately
        
        //Debug.Log($"{name} starts attacking {target.name}");
    }

    private void StopAutoAttacking()
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
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
    }

    #endregion

    #region Casting

    protected void BeginCasting(string abilityName, float castTime)
    {
        if (!CanAct()) return;

        isCasting = true;
        currentCastName = abilityName;
        currentCastTime = castTime;
        currentCastProgress = 0;
        OnCastStart?.Invoke(abilityName,castTime);
    }

    protected void UpdateCastProgress(float progress)
    {
        currentCastProgress = progress;
    }

    protected void CompleteCast()
    {
        isCasting = false;
        OnCastComplete?.Invoke(currentCastName);

        autoAttackTimer = 1f / attacksPerSecond;
    }

    protected void InterruptCast()
    {
        isCasting = false;
        OnCastInterrupted?.Invoke(currentCastName);
    }

    #endregion

    public bool CanAct()
    {
        return !isDead && !isCasting;
    }

}

public enum EntityType
{
    Player,
    Boss
}