using UnityEngine;

[CreateAssetMenu(fileName = "New DoT Ability", menuName = "Combat/Special Abilities/DoT Ability")]
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
