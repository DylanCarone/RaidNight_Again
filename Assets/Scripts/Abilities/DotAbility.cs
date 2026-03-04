using UnityEngine;

[CreateAssetMenu(fileName = "New DoT Ability", menuName = "Combat/Special Abilities/DoT/DoT Ability")]
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

[CreateAssetMenu(fileName = "New DoT-Stack Ability", menuName = "Combat/Special Abilities/DoT/DoT with Stack Bonus Ability")]
public class DoTWithStackBonusAbility  : DoTAbility
{
    [Header("Stack Bonus Config")]
    public string stackBonusEffectName = "DoTStackBonus";
    [Range(0,1)] public float bonusStackChance = 0.5f;

    public int bonusStackCount = 1;
    


    public override void ExecuteAbility(CombatEntity caster, CombatEntity target)
    {
        // apply instant Damage
        base.ExecuteAbility(caster, target);
        
        ApplyStackBonusToPlayer(caster);
    }
    
    private void ApplyStackBonusToPlayer(CombatEntity caster)
    {
        StatusEffectManager playerManager = caster.StatusEffectManager;
        if (playerManager == null)
        {
            Debug.LogWarning($"[DoTWithStackBonus] {abilityName}: caster has no StatusEffectManager.");
            return;
        }

        // If already active, refresh it (so recasting the DoT refreshes the bonus too)
        StatusEffect existing = playerManager.GetEffectByName(stackBonusEffectName);
        if (existing != null)
        {
            existing.Refresh();
            Debug.Log($"[DoTWithStackBonus] Refreshed {stackBonusEffectName} on {caster.name}");
            return;
        }

        float duration = dotDuration;

        // The StackBonusEffect lives on the player (caster), targeting themselves
        GameObject effectObj = new GameObject($"StackBonus_{stackBonusEffectName}");
        effectObj.transform.SetParent(caster.transform);

        StackBonusEffect bonusEffect = effectObj.AddComponent<StackBonusEffect>();
        bonusEffect.Initialize(caster, caster, stackBonusEffectName, duration, bonusStackChance, bonusStackCount);

        playerManager.activeEffects.Add(bonusEffect);

        Debug.Log($"[DoTWithStackBonus] Applied {stackBonusEffectName} to {caster.name} for {duration}s");
    }
}
