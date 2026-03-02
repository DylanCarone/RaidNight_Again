using System.Collections;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    [Header("Basic Info")] 
    protected string effectName;
    protected float duration;
    protected float remainingDuration;
    protected CombatEntity caster;
    protected CombatEntity target;

    [Header("Visual")] 
    protected GameObject visualEffectPrefab;
    protected GameObject visualEffectInstance;
    
    public string EffectName => effectName;
    public float RemainingDuration => remainingDuration;
    public CombatEntity Caster => caster;

    public virtual void Initialize(CombatEntity caster, CombatEntity target, string name, float duration)
    {
        this.caster = caster;
        this.target = target;
        this.effectName = name;
        this.duration = duration;
        this.remainingDuration = duration;
        
        Debug.Log($"Applied {effectName} to {target.name} for {duration}s");
        
        OnApplied();
        StartCoroutine(TickCoroutine());
    }

    public virtual void Refresh()
    {
        remainingDuration = duration;
        OnRefresh();
    }
    
    protected abstract void OnApplied();
    protected abstract void OnRefresh();
    protected abstract void OnTick();
    protected abstract void OnExpired();
    protected abstract void OnRemoved();
    protected abstract float GetTickRate();

    private IEnumerator TickCoroutine()
    {
        float timeSinceLastTick = 0f;
        float tickRate = GetTickRate();

        while (remainingDuration > 0f && target != null && !target.IsDead)
        {
            yield return null;

            float deltaTime = Time.deltaTime;
            remainingDuration -= deltaTime;
            timeSinceLastTick += deltaTime;

            if (timeSinceLastTick >= tickRate)
            {
                OnTick();
                timeSinceLastTick = 0f;
            }
        }

        if (remainingDuration <= 0f)
        {
            OnExpired();
            Debug.Log($"{effectName} expired on {target.name}");
        }
        
        RemoveEffect();
    }


    public void RemoveEffect()
    {
        OnRemoved();
        
        StopAllCoroutines();
        
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();
        if(manager != null)
            manager.RemoveEffect(this);
        
        if(visualEffectInstance != null)
            Destroy(visualEffectInstance);
        
        // notify the statusEffectManager and remove this effect
    }
    
    

    // on applied
    // on refresh
    // on tick
    // on expired
    // on removed
    // get tick rate
    // TickCoroutine


}
