using UnityEngine;
using System;

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





