using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityInstance
{
    public Ability ability;
    private float currentCooldown;

    private bool canCast;
    public bool CanCast => canCast;

    private bool isEmpowered = false;

    private List<AbilityEmpowerment> activeEmpowerments = new List<AbilityEmpowerment>();
    public IReadOnlyList<AbilityEmpowerment> ActiveEmpowerments => activeEmpowerments;
    public bool IsEmpowered => activeEmpowerments.Count > 0;
    
    public event Action<List<AbilityEmpowerment>> OnEmpowermentsChanged;

    public AbilityInstance(Ability ability)
    {
        this.ability = ability;
        currentCooldown = 0f;
    }

    public bool IsReady()
    {
        return currentCooldown <= 0f;
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, currentCooldown);
    }

    public float GetCooldownPercent()
    {
        if (ability.cooldown <= 0) return 0f;
        
        return Mathf.Clamp01(currentCooldown / ability.cooldown);
    }

    public void StartCooldown()
    {
        currentCooldown = ability.cooldown;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
        }
    }

    public void ResetCooldown()
    {
        currentCooldown = 0f;
    }
    
    public void SetEmpowered(bool value) => isEmpowered = value;

    public void ApplyEmpowerment(AbilityEmpowerment empowerment)
    {
        activeEmpowerments.Add(empowerment);
        OnEmpowermentsChanged?.Invoke(activeEmpowerments);
    }
    
    public void ConsumeEmpowerments()
    {
        activeEmpowerments.Clear();
        OnEmpowermentsChanged?.Invoke(activeEmpowerments);
    }

    public float GetModifiedResourceCost()
    {
        float cost = ability.resourceCost;
        foreach (var e in activeEmpowerments)
        {
            cost = e.ModifyResourceCost(cost);
        }

        return cost;
    }
    
    public float GetModifiedResourceGained()
    {
        float gain = ability.resourceGain;
        foreach (var e in activeEmpowerments)
        {
            gain = e.ModifyResourceGain(gain);
        }

        return gain;
    }
    public float GetModifiedCastTime()
    {
        float castTime = ability.castTime;
        foreach (var e in activeEmpowerments)
            castTime = e.ModifyCastTime(castTime);
        
        return castTime;
    }

    public float GetModifiedDamage()
    {
        float damage = ability.damage;
        foreach (var e in activeEmpowerments)
            damage = e.ModifyDamage(damage);
        return damage;
    }
}
