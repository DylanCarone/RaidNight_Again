using System;
using System.Collections;
using UnityEngine;

public class GolemBoss : BossCombatEntity
{
    [Header("Ability Data")]
    [SerializeField] private BossAbility raidWide;
    [SerializeField] private BossAbility slam;
    [SerializeField] private BossAbility blastoff;
    [SerializeField] private BossAbilityTelegraph shockWave;
    [SerializeField] private BossAbilityTelegraph overdrive;


    protected override IEnumerator BossRotation()
    {
        yield return new WaitForSeconds(2f);
        while (!IsDead)
        {
            yield return UseAttack(slam);
            yield return new WaitForSeconds(4f);
            yield return UseAttack(shockWave);
            yield return new WaitForSeconds(4f);
            
        }
    }

    

}
