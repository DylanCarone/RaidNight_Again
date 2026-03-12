using System.Collections;
using UnityEngine;

public abstract class AbilityEmpowerment : ScriptableObject
{
    public string empowermentName;
    public Sprite empowermentIcon;
    
    public virtual float ModifyResourceCost(float originalCost) => originalCost;
    public virtual float ModifyResourceGain(float originalGain) => originalGain;
    public virtual float ModifyDamage(float originalDamage) => originalDamage;
    public virtual float ModifyCastTime(float originalCastTime) => originalCastTime;

    public virtual void OnSpellFired(PlayerCombatEntity caster, CombatEntity target) =>
        Debug.Log("Empowered Spell Fired");

}











