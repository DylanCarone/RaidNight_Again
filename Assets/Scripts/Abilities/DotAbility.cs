using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New DoT Ability", menuName = "Combat/Special Abilities/DoT/DoT Ability")]
public class DoTAbility : Ability
{
    [Header("Damage over Time")] 
    public float damagePerTick = 20f;
    public float dotDuration = 10f;
    public float dotTickRate = 1f;
    public GameObject visualEffectPrefab;


    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        // apply instant Damage
        base.ExecuteAbility(caster, target);
        
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();
        if (manager != null)
        {
            manager.ApplyDoT(caster, target, this);
        }
    }
}

[CreateAssetMenu(fileName = "New DoT-Stack Ability", menuName = "Combat/Special Abilities/DoT/DoT with Stack Bonus Ability")]
public class DoTWithStackBonusAbility  : DoTAbility
{
    
    [Range(0,1)] public float bonusStackChance = 0.5f;

    public int bonusStackCount = 1;
    


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
        StatusEffect existing = playerManager.GetEffectByName(abilityName);
        if (existing != null)
        {
            existing.Refresh();
            Debug.Log($"[DoTWithStackBonus] Refreshed {abilityName} on {caster.name}");
            return;
        }

        float duration = dotDuration;

        // The StackBonusEffect lives on the player (caster), targeting themselves
        GameObject effectObj = new GameObject($"StackBonus_{abilityName}");
        effectObj.transform.SetParent(caster.transform);

        StackBonusEffect bonusEffect = effectObj.AddComponent<StackBonusEffect>();
        bonusEffect.Initialize(caster, caster, abilityName, duration, bonusStackChance, bonusStackCount);

        playerManager.activeEffects.Add(bonusEffect);

        Debug.Log($"[DoTWithStackBonus] Applied {abilityName} to {caster.name} for {duration}s");
    }
}

[CreateAssetMenu(fileName = "New DoT-Proc Ability", menuName = "Combat/Special Abilities/DoT/DoT with Proc Ability")]
public class DotProcAbility  : DoTAbility, IStackingAbility
{
    [Header("Proc Config")] 
    [Range(0f, 1f)] public float procChance = 0.2f;
    public Ability abilityToBuff;
    public List<AbilityEmpowerment> empowerments;
    public int maxStacks = 3;
    
    
    public Ability AbilityToBuff=>abilityToBuff;
    public int MaxStacks=> maxStacks;

    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        PlayerCombatEntity player = caster as PlayerCombatEntity;
        if (player == null)
        {
            // apply instant Damage
            base.ExecuteAbility(caster, target);
            return;
        }
        
        StatusEffectManager manager = target.StatusEffectManager;
        if (manager == null) return;
        
        manager.ApplyDotProc(caster, target, this, player);


    }
    

    public int GetCurrentStacks(PlayerCombatEntity player)
    {
        if (empowerments == null || empowerments.Count == 0) return 0;
        
        AbilityInstance targetSpell = player.GetSpellByAbility(abilityToBuff);
        if (targetSpell == null) return 0;

        AbilityEmpowerment stackAnchor = empowerments[0];
        return targetSpell.ActiveEmpowerments.Count(e => e == stackAnchor);
    }
    private int CalculateStacksToApply(PlayerCombatEntity player)
    {
        // Check if the player has any active StackBonusEffect
        StackBonusEffect bonusEffect = player.StatusEffectManager
            .GetEffectsOfType<StackBonusEffect>()
            .FirstOrDefault();

        if (bonusEffect != null)
        {
            // Delegate the roll to the effect itself — it owns the chance logic
            return bonusEffect.RollTotalStacks();
        }

        return 1; // Default: always apply 1 stack
    }

}
