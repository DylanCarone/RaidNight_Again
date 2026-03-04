using UnityEngine;

public class StackBonusEffect : StatusEffect
{
    
    private float bonusStackChance = 0.5f;
    private int bonusStacksApplied = 0;
    
    public int BonusBonusStackCount => bonusStacksApplied;
    public float BonusStackChance => bonusStackChance;

    public void Initialize(CombatEntity caster, CombatEntity target, string name, float duration,
        float bonusStackChance, int bonusStackCount)
    {
        this.bonusStackChance = bonusStackChance;
        this.bonusStacksApplied = bonusStackCount;
        
        base.Initialize(caster, target, name, duration);;
    }
    
    protected override float GetTickRate() => float.MaxValue;

    public int RollTotalStacks()
    {
        bool proceed = Random.value <= bonusStackChance;
        int total = 1 + (proceed ? bonusStacksApplied : 0);
        return total;
    }



}
