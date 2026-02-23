using UnityEngine;

[CreateAssetMenu(fileName = "New Resurrect Ability", menuName = "Combat/Special Abilities/Resurrect Ability")]
public class ResAbility : Ability
{
    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        if (target == null) return;
        
        PlayerCombatEntity player = target as PlayerCombatEntity;

        if (player != null && player.IsDead)
        {
            player.Resurrect(healing);
        }
    }
}
