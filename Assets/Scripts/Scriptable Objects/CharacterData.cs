using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Settings")]
    public GameObject playerModel;
    public PlayerRole role;
    public string className;
    [Header("Combat")]
    public float maxHealth;
    public float attackSpeed;
    public float attackDamage;
    public float attackRange;
    public Ability[] abilities;
    [Header("Resources")]
    public ResourceType resourceType;
    
}


public abstract class ResourceType : ScriptableObject
{
    [Header("Base Settings")] 
    public float maxResource;
    public float regenPerSecond;
    public virtual bool StartFull => false;
    public virtual bool AutoRegenerate => false;
    public virtual bool RestoreOnAutoAttack => false;
    public virtual bool RestoreResourceOnHit => false;
    public virtual Color ResourceColor => Color.white;

    public virtual bool IsThresholdActive(float current, float max)
    {
        return false;
    }

    public virtual float GetEffectiveGCD(float baseGCD, bool thresholdMet)
    {
        return baseGCD;
    }

}

[CreateAssetMenu(fileName = "New Mana Resource", menuName = "Game/Resource Type/Mana")]
public class ManaResource : ResourceType
{
    
    public override bool StartFull => true;
    public override bool AutoRegenerate => true;
    public override Color ResourceColor => MyPalette.ManaBlue;
}

[CreateAssetMenu(fileName = "New Adrenaline Resource", menuName = "Game/Resource Type/Adrenaline")]
public class AdrenalineResource : ResourceType
{
    [Range(0, 1)] public float frenzyThreshold = 0.6f;
    public float frenzyGCD = 1.5f;
    public override Color ResourceColor => MyPalette.EnergyYellow;

    public override bool IsThresholdActive(float current, float max)
    {
        // at 60% of max, player should enter a state of frenzy,
        // which reduces the GCD to 1.5s and increase auto attack speed
        return current >= max * frenzyThreshold;
    }

    public override float GetEffectiveGCD(float baseGCD, bool thresholdMet)
    {
        if(thresholdMet)
            return frenzyGCD;
        return baseGCD;
        
    }
}


