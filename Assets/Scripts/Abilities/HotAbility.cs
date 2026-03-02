using UnityEngine;


[CreateAssetMenu(fileName = "New HoT Ability", menuName = "Combat/Special Abilities/HoT Ability")]
public class HotAbility : Ability
{
    [Header("Healing Over Time")] 
    public float healingPerTick = 20f;

    public float hotDuration = 10f;
    public float hotTickRate = 1f;
    public GameObject visualEffectPrefab;

    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        base.ExecuteAbility(caster, target);
        
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();
        if (manager != null)
        {
            manager.ApplyHoT(caster, target, this);
        }
    }
}
