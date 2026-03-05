using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "New Proc Ability", menuName = "Combat/Special Abilities/Proc Ability")]
public class ProcAbility : Ability, IStackingAbility
{
    [Header("Proc Config")] [Range(0f, 1f)] public float procChance = 0.2f;
    public Ability abilityToBuff;
    public List<AbilityEmpowerment> empowerments;

    public int maxStacks = 3;
    
    public Ability AbilityToBuff=>abilityToBuff;
    public int MaxStacks=> maxStacks;

    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        base.ExecuteAbility(caster, target);
        
        PlayerCombatEntity player = caster as PlayerCombatEntity;
        if (player == null) return;
        
        AbilityInstance buffedSpell = player?.GetSpellByAbility(abilityToBuff);
        if (buffedSpell == null) return;

        if (Random.value > procChance) return;
        
        int currentStacks = GetCurrentStacks(player);
        
        int stacksToApply = CalculateStacksToApply(player);
        
        int allowedStacks = Mathf.Min(maxStacks - currentStacks, stacksToApply);

        if (allowedStacks <=0)
        {
            Debug.Log($"[ProcAbility] {abilityName}: already at max stacks ({currentStacks}/{maxStacks})");
            return;
        }

        for (int i = 0; i < allowedStacks; i++)
        {
            foreach (var e in empowerments)
            {
                buffedSpell.ApplyEmpowerment(e);
            }
        }
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
    
    public int GetCurrentStacks(PlayerCombatEntity player)
    {
        if (empowerments == null || empowerments.Count == 0) return 0;
        
        AbilityInstance targetSpell = player.GetSpellByAbility(abilityToBuff);
        if (targetSpell == null) return 0;

        AbilityEmpowerment stackAnchor = empowerments[0];
        return targetSpell.ActiveEmpowerments.Count(e => e == stackAnchor);
    }
}
