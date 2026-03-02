using UnityEngine;
public class HotEffect : StatusEffect
{
    private float healingPerTick;
    private float tickRate;

    public void Initialize(CombatEntity caster, CombatEntity target, string name, float duration,
        float healingPerTick, float tickRate, GameObject effectPrefab)
    {
        this.healingPerTick = healingPerTick;
        this.tickRate = tickRate;
        this.visualEffectPrefab = effectPrefab;
        this.target = target;
        
        base.Initialize(caster, target, name, duration);
    }
    
    protected override void OnApplied()
    {
        if (visualEffectPrefab != null)
        {
            visualEffectInstance = Instantiate(visualEffectPrefab, target.transform.position, Quaternion.identity, target.transform);

        }
    }

    protected override void OnRefresh()
    {
        
    }

    protected override void OnTick()
    {
        Debug.Log($"Healed {target.name} for {healingPerTick} hp");
        target.Heal(healingPerTick);
    }

    protected override void OnExpired()
    {
        Destroy(visualEffectInstance);
    }

    protected override void OnRemoved()
    {
        Destroy(visualEffectInstance);
    }

    protected override float GetTickRate()
    {
        return tickRate;
    }
}
