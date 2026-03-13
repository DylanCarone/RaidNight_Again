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
    protected Sprite icon;
    public Sprite Icon => icon;
    public float Duration => duration;

    [Header("Visual")] 
    protected GameObject visualEffectPrefab;
    protected GameObject visualEffectInstance;
    
    public string EffectName => effectName;
    public float RemainingDuration => remainingDuration;
    public CombatEntity Caster => caster;

    protected virtual void Initialize(CombatEntity caster, CombatEntity target, string name, float duration, Sprite icon = null)
    {
        this.caster = caster;
        this.target = target;
        this.effectName = name;
        this.duration = duration;
        this.remainingDuration = duration;
        this.icon = icon;
        
        Debug.Log($"Applied {effectName} to {target.name} for {duration}s");
        
        OnApplied();
        StartCoroutine(TickCoroutine());
    }

    public virtual void Refresh()
    {
        remainingDuration = duration;
        OnRefresh();
    }

    protected virtual void OnApplied()
    {
    }

    protected virtual void OnRefresh()
    {
    }

    protected virtual void OnTick()
    {
    }

    protected virtual void OnExpired()
    {
    }

    protected virtual void OnRemoved()
    {
    }

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
        
        StopAllCoroutines();
        
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();
        if(manager != null)
            manager.RemoveEffect(this);
        
        if(visualEffectInstance != null)
            Destroy(visualEffectInstance);
        
        OnRemoved();
        
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
