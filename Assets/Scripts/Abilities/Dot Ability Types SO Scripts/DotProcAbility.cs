using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New DoT-Proc Ability", menuName = "Combat/Special Abilities/DoT/DoT with Proc Ability")]
public class DotProcAbility  : DotAbility, IStackingAbility
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
