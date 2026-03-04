using System.Collections;
using UnityEngine;

public abstract class AbilityEmpowerment : ScriptableObject
{
    public string empowermentName;
    public Sprite empowermentIcon;
    
    public virtual float ModifyResourceCost(float originalCost) => originalCost;
    public virtual float ModifyDamage(float originalDamage) => originalDamage;
    public virtual float ModifyCastTime(float originalCastTime) => originalCastTime;

    public virtual void OnSpellFired(PlayerCombatEntity caster, CombatEntity target) =>
        Debug.Log("Empowered Spell Fired");

}


// Free cast
[CreateAssetMenu(menuName = "Combat/Empowerments/Free Cast")]
public class FreeCastEmpowerment : AbilityEmpowerment
{
    public override float ModifyResourceCost(float originalCost) => 0f;
}

// Instant cast
[CreateAssetMenu(menuName = "Combat/Empowerments/Instant Cast")]
public class InstantCastEmpowerment : AbilityEmpowerment
{
    public override float ModifyCastTime(float originalCastTime) => 0f;
}

// Damage amplifier
[CreateAssetMenu(menuName = "Combat/Empowerments/Amplify")]
public class AmplifyEmpowerment : AbilityEmpowerment
{
    public float multiplier = 2f;
    public override float ModifyDamage(float originalDamage) => originalDamage * multiplier;
}

// Double cast - fires the spell twice
[CreateAssetMenu(menuName = "Combat/Empowerments/Double Cast")]
public class DoubleCastEmpowerment : AbilityEmpowerment
{
    public override void OnSpellFired(PlayerCombatEntity caster, CombatEntity target)
    {
        // second hit after a short delay
        caster.StartCoroutine(DelayedHit(caster, target));
    }

    private IEnumerator DelayedHit(PlayerCombatEntity caster, CombatEntity target)
    {
        yield return new WaitForSeconds(0.3f);
        target.TakeDamage(50f);
    }
}