using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Combat/Ability")]
public class Ability : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName = "New ability";
    public Sprite icon;
    [TextArea(2,4)]
    public string description = "Ability description";
    
    public SpecialAnimations animation;

    public ResourceType resourceType;
    public float resourceCost;
    
    public AbilityTargetType targetType;
    public float damage;
    public float healing;
    public float castTime;
    public float cooldown;
    public float range;
    public bool isOnGDC = true;

    private float timer;

    public virtual void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        if (damage > 0 && target != null && !target.IsDead)
        {
            target.TakeDamage(damage);
        }

        if (healing > 0)
        {
            CombatEntity healTarget = targetType == AbilityTargetType.Self ? caster : target;
            healTarget.Heal(healing);
        }
        
        
    }

}


public enum AbilityTargetType
{
    Self,
    Ally,
    Boss
}