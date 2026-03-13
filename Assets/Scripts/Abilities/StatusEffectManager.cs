using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public  List<StatusEffect> activeEffects = new List<StatusEffect>();
    public List<StatusEffect> ActiveEffects => activeEffects;
    
    public event Action<StatusEffect> OnEffectApplied;
    public event Action<StatusEffect> OnEffectRemoved;


    public StatusEffect GetEffectByName(string effectName)
    {
        return activeEffects.FirstOrDefault(e => e.EffectName == effectName);
    }
    


    public void ApplyDoT(CombatEntity caster, CombatEntity target, DotAbility ability)
    {
        StatusEffect existingEffect = GetEffectByName(ability.abilityName);
        
        if (existingEffect != null)
        {
            // refresh the current effect
            existingEffect.Refresh();
            return;
        }
        
        GameObject effectObj = new GameObject($"DoT_{ability.abilityName}");
        effectObj.transform.SetParent(target.transform);
        
        DotEffect dot = effectObj.AddComponent<DotEffect>();
        dot.Initialize(caster, target, ability.abilityName, ability.dotDuration, ability.damagePerTick, ability.dotTickRate, ability.visualEffectPrefab, ability.icon);
        
        activeEffects.Add(dot);
        OnEffectApplied?.Invoke(dot);
    }

    public void ApplyDotProc(CombatEntity caster, CombatEntity target, DotProcAbility ability,
        PlayerCombatEntity player)
    {
        StatusEffect existingEffect = GetEffectByName(ability.abilityName);
        if (existingEffect != null)
        {
            existingEffect.Refresh();
            return;
        }
        
        GameObject effectObj = new GameObject($"DoT_{ability.abilityName}");
        effectObj.transform.SetParent(target.transform);
        
        DotProcEffect dot = effectObj.AddComponent<DotProcEffect>();
        
        dot.SetProcConfig(player, ability.abilityToBuff, ability.empowerments, ability.procChance, ability.maxStacks);
        dot.Initialize(caster, target, ability.abilityName, ability.dotDuration, ability.damagePerTick, ability.dotTickRate, ability.visualEffectPrefab, ability.icon);
        activeEffects.Add(dot);
        OnEffectApplied?.Invoke(dot);
    }

    public void ApplyHoT(CombatEntity caster, CombatEntity target, HotAbility ability)
    {
        StatusEffect existingEffect = GetEffectByName(ability.abilityName);

        if (existingEffect != null)
        {
            existingEffect.Refresh();
            return;
        }
        
        GameObject effectObj = new GameObject($"HoT_{ability.abilityName}");
        effectObj.transform.SetParent(target.transform);
        
        HotEffect hot = effectObj.AddComponent<HotEffect>();
        hot.Initialize(caster, target, ability.abilityName, ability.hotDuration,  ability.healingPerTick,
            ability.hotTickRate, ability.visualEffectPrefab);
        activeEffects.Add(hot);
        OnEffectApplied?.Invoke(hot);
        
    }

    public void ApplyBuff(CombatEntity caster, CombatEntity target, BuffAbility ability)
    {
        StatusEffect existingEffect = GetEffectByName(ability.abilityName);
        if (existingEffect != null)
        {
            existingEffect.Refresh();
            return;
        }
        
        GameObject effectObj = new GameObject($"Buff_{ability.abilityName}");
        effectObj.transform.SetParent(target.transform);
        
        BuffEffect buff = effectObj.AddComponent<BuffEffect>();
        buff.Initialize(caster, target,  ability.abilityName, ability.buffDuration,ability.buffType, ability.buffAmount, ability.visualEffectPrefab, ability.icon);
        activeEffects.Add(buff);
        OnEffectApplied?.Invoke(buff);
    }

    public bool HasEffect(string effectName)
    {
        return GetEffectByName(effectName) != null;
    }

    public void RemoveEffect(StatusEffect effect)
    {
        activeEffects.Remove(effect);
        OnEffectRemoved?.Invoke(effect);
        
    }

    public void RemoveAllEffects()
    {
        List<StatusEffect> toRemove = new List<StatusEffect>(activeEffects);

        foreach (StatusEffect effect in toRemove)
        {
            effect.RemoveEffect();
            
        }
        activeEffects.Clear();
    }
    
    public List<T> GetEffectsOfType<T>() where T : StatusEffect
    {
        return activeEffects.OfType<T>().ToList();
    }
    
}
