using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Ability", menuName = "Combat/Special Abilities/BuffAbility")]
public class BuffAbility : Ability
{
    [Header("Buff")]
    public BuffType buffType;

    public float buffAmount = 1f;
    public float buffDuration = 10f;
    public GameObject visualEffectPrefab;

    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        base.ExecuteAbility(caster, target);
        switch (targetType)
        {
            case AbilityTargetType.Self:
                target = caster;
                break;
        }
        
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();
        if (manager != null)
        {
            manager.ApplyBuff(caster, target, this);
        }
        
        
    }
}
