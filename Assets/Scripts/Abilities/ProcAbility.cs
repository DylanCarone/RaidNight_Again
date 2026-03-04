using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Proc Ability", menuName = "Combat/Special Abilities/Proc Ability")]
public class ProcAbility : Ability
{
    [Header("Proc")] [Range(0f, 1f)] public float procChance = 0.2f;
    public Ability abilityToBuff;
    public List<AbilityEmpowerment> empowerments;

    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        base.ExecuteAbility(caster, target);

        if (Random.value > procChance) return;

        PlayerCombatEntity player = caster as PlayerCombatEntity;
        AbilityInstance buffedSpell = player?.GetSpellByAbility(abilityToBuff);
        foreach (var e in empowerments)
        {
            buffedSpell?.ApplyEmpowerment(e);
        }
    }
}
