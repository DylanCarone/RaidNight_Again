using System;
using UnityEngine;
public class BuffEffect : StatusEffect
{
    private BuffType buffType;
    private float amount;
    private float currentShieldAmount; // for shield buffs
    
    public BuffType BuffType => buffType;
    public float Amount => amount;
    
    public  float CurrentShieldAmount => currentShieldAmount;

    public Action<bool> OnShieldChanged;


    public void Initialize(CombatEntity caster, CombatEntity target, string name, float duration,
        BuffType buffType, float buffAmount, GameObject effectPrefab)
    {
        
        this.buffType = buffType;
        this.amount = buffAmount;
        this.visualEffectPrefab = effectPrefab;
        this.duration = duration;
        
        if(buffType == BuffType.Shield)
            currentShieldAmount = amount;
        
        base.Initialize(caster, target, name, duration);
    }

    protected override void OnApplied()
    {
        ApplyBuff();
        if (visualEffectPrefab != null)
        {
            visualEffectInstance = Instantiate(visualEffectPrefab, target.transform.position + Vector3.down, Quaternion.identity, caster.transform);
        }
    }

    protected override void OnRefresh()
    {
        if (buffType == BuffType.Shield)
        {
            currentShieldAmount = amount;
        }
    }

    protected override void OnTick()
    {
        if (buffType == BuffType.ResourceRegeneration && target is PlayerCombatEntity player)
        {
            player.RestoreResource(amount);
            Debug.Log($"{effectName} restores {amount} resource to {player.name}");
        }
    }

    protected override void OnExpired()
    {
        RemoveBuff();
        
    }

    protected override void OnRemoved()
    {
        RemoveBuff();
    }

    protected override float GetTickRate()
    {
        if (buffType == BuffType.ResourceRegeneration)
            return 1f;
        
        // most buffs dont tick
        return float.MaxValue;
    }

    private void ApplyBuff()
    {
        if (target == null) return;

        switch (buffType)
        {
            case BuffType.DamageReduction:
                target.SetDamageReduction(amount);
                break;
            case BuffType.Shield:
                Debug.Log($"🛡️ {target.name} gained {amount} shield");
                OnShieldChanged?.Invoke(true);
                break;
                
        }
    }
    
    private void RemoveBuff()
    {
        if (target == null) return;

        switch (buffType)
        {
            case BuffType.DamageReduction:
                target.ResetDamageReduction(amount);
                break;
            case BuffType.Shield:
                Debug.Log($"{target.name} lost the shield");
                OnShieldChanged?.Invoke(false);
                break;
        }
    }

    /// <summary>
    /// Absorb damage with shield (called from TakeDamage)
    /// </summary>
    public float AbsorbDamage(float incomingDamage)
    {
        if (buffType != BuffType.Shield) return incomingDamage;
        if (currentShieldAmount >= incomingDamage)
        {
            currentShieldAmount -= incomingDamage;
            return 0f;
        }
        else
        {
            float remainingDamage = incomingDamage - currentShieldAmount;
            currentShieldAmount = 0f;
            RemoveEffect();
            return remainingDamage;
        }
    }
}
