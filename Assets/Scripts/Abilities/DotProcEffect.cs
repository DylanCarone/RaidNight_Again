using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotProcEffect : DotEffect
{
    private PlayerCombatEntity casterPlayer;
    private Ability abilityToBuff;
    List<AbilityEmpowerment> empowerments;
    private float procChance;
    private int maxStacks;
    
    public void SetProcConfig(PlayerCombatEntity casterPlayer, Ability abilityToBuff, List<AbilityEmpowerment> empowerments, float procChance, int maxStacks)
    {
        this.casterPlayer = casterPlayer;
        this.abilityToBuff = abilityToBuff;
        this.empowerments = empowerments;
        this.procChance = procChance;
        this.maxStacks = maxStacks;
    }

    protected override void OnTick()
    {
        base.OnTick();
        TryProc();
        
    }

    private void TryProc()
    {
        if (casterPlayer == null) return;
        if (Random.value > procChance) return;

        AbilityInstance buffedSpell = casterPlayer.GetSpellByAbility(abilityToBuff);
        if(buffedSpell == null) return;

        int currentStacks = GetCurrentStacks(buffedSpell);
        int stacksToApply = CalculateStacksToApply();
        int allowedStacks = Mathf.Min(maxStacks - currentStacks, stacksToApply);
        if (allowedStacks <= 0) return;
        
        for (int i = 0; i < allowedStacks; i++)
        {
            foreach (var e in empowerments)
                buffedSpell.ApplyEmpowerment(e);
        }
    }
    
    public int GetCurrentStacks(AbilityInstance buffedSpell)
    {
        if (empowerments == null || empowerments.Count == 0) return 0;
        return Enumerable.Count(buffedSpell.ActiveEmpowerments, e => e == empowerments[0]);
    }
    private int CalculateStacksToApply()
    {
        // Check if the player has any active StackBonusEffect
        StackBonusEffect bonusEffect = casterPlayer.StatusEffectManager
            .GetEffectsOfType<StackBonusEffect>()
            .FirstOrDefault();
        return bonusEffect != null ? bonusEffect.RollTotalStacks() : 1;
    }
}