using UnityEngine;


[CreateAssetMenu(fileName = "New DoT-Stack Ability", menuName = "Combat/Special Abilities/DoT/DoT with Stack Bonus Ability")]
public class DotWithStackBonusAbility  : DotAbility
{
    
    [Range(0,1)] public float bonusStackChance = 0.5f;

    public int bonusStackCount = 1;
    public AbilityEmpowerment stackEmpowerment;
    


    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        // apply instant Damage
        base.ExecuteAbility(caster, target);
        
        ApplyStackBonusToPlayer(caster);
    }
    
    private void ApplyStackBonusToPlayer(CombatEntity caster)
    {
        StatusEffectManager playerManager = caster.StatusEffectManager;
        if (playerManager == null)
        {
            Debug.LogWarning($"[DoTWithStackBonus] {abilityName}: caster has no StatusEffectManager.");
            return;
        }

        // If already active, refresh it (so recasting the DoT refreshes the bonus too)
        StatusEffect existing = playerManager.GetEffectByName(stackEmpowerment.empowermentName);
        if (existing != null)
        {
            existing.Refresh();
            Debug.Log($"[DoTWithStackBonus] Refreshed {stackEmpowerment.empowermentName} on {caster.name}");
            return;
        }

        float duration = dotDuration;

        // The StackBonusEffect lives on the player (caster), targeting themselves
        GameObject effectObj = new GameObject($"StackBonus_{stackEmpowerment.empowermentName}");
        effectObj.transform.SetParent(caster.transform);

        StackBonusEffect bonusEffect = effectObj.AddComponent<StackBonusEffect>();
        bonusEffect.Initialize(caster, caster, stackEmpowerment.empowermentName, duration, bonusStackChance, bonusStackCount);

        playerManager.activeEffects.Add(bonusEffect);

        Debug.Log($"[DoTWithStackBonus] Applied {stackEmpowerment.empowermentName} to {caster.name} for {duration}s");
    }
}