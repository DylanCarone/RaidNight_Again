using UnityEngine;

public class DotEffect : StatusEffect
{
    private float damagePerTick;
    private float tickRate;
    

    public void Initialize(CombatEntity caster, CombatEntity target, string name, 
        float duration, float damagePerTick,
        float tickRate, GameObject effectPrefab, Sprite icon)
    {
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.visualEffectPrefab = effectPrefab;
        this.target = target;
        
        base.Initialize(caster, target, name, duration, icon);
    }

    protected override void OnApplied()
    {
        // can play a sound, spawn particles, etc.
        if (visualEffectPrefab != null)
        {
            visualEffectInstance = Instantiate(visualEffectPrefab, target.transform.position + Vector3.up * 1.5f, Quaternion.identity, target.transform);
        }
    }

    protected override void OnRefresh()
    {
        Debug.Log($"Refreshed dot effect");
    }

    protected override void OnTick()
    {
        target.TakeDamage(damagePerTick);
        
    }

    protected override void OnExpired()
    {
        Debug.Log($"Dot Expired");
        Destroy(visualEffectInstance);
    }

    protected override void OnRemoved()
    {
        Debug.Log($"Dot Removed");
        Destroy(visualEffectInstance);
    }

    protected override float GetTickRate()
    {
        return tickRate;
    }
}
