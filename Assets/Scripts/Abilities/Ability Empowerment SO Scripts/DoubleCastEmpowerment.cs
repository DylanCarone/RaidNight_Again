using System.Collections;
using UnityEngine;


// Double cast - fires the spell twice
[CreateAssetMenu(menuName = "Combat/Empowerments/Double Cast")]
public class DoubleCastEmpowerment : AbilityEmpowerment
{
    public override void OnSpellFired(PlayerCombatEntity caster, CombatEntity target)
    {
        // second hit after a short delay
        caster.StartCoroutine(DelayedHit(caster, target));
    }

    private IEnumerator DelayedHit(PlayerCombatEntity caster, CombatEntity target)
    {
        yield return new WaitForSeconds(0.3f);
        target.TakeDamage(50f);
    }
}