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

[CreateAssetMenu(menuName = "Combat/Empowerments/Cost Reduction Cast")]
public class CostReductionEmpowerment : AbilityEmpowerment
{
    [Tooltip("Percentage of the original cost to reduce to.")]
    [Range(0,100)] public float reductionPercentage = 0f;
    public override float ModifyResourceCost(float originalCost) => originalCost * (1f - reductionPercentage / 100f);
}

// Instant cast
[CreateAssetMenu(menuName = "Combat/Empowerments/Instant Cast")]
public class InstantCastEmpowerment : AbilityEmpowerment
{
    public override float ModifyCastTime(float originalCastTime) => 0f;
}

[CreateAssetMenu(menuName = "Combat/Empowerments/Cast Reduction")]
public class CastReductionEmpowerment : AbilityEmpowerment
{
    public float flatReduction = 0f;
    public override float ModifyCastTime(float originalCastTime) => originalCastTime - flatReduction;
}

// Damage amplifier
[CreateAssetMenu(menuName = "Combat/Empowerments/Amplify")]
public class AmplifyEmpowerment : AbilityEmpowerment
{
    [Tooltip("Flat damage to apply to the target. Multiplier is applied after this.")]
    public float flatDamageIncrease = 50f;
    [Tooltip("Multiplier to apply to the damage.")]
    public float multiplier = 1f;
    public override float ModifyDamage(float originalDamage) => (originalDamage + flatDamageIncrease) * multiplier;
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