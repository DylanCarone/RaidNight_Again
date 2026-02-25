using UnityEngine;

[CreateAssetMenu(fileName = "New Interrupt Ability", menuName = "Combat/Special Abilities/InterruptAbility")]
public class InterruptAbility : Ability
{
    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        if (target == null) return;
        
        BossCombatEntity boss = target as BossCombatEntity;

        if (boss != null && boss.IsCasting)
        {
            boss.InterruptCurrentCast();
        }
    }
}
