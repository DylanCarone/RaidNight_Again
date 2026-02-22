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
        while (!IsDead)
        {
            
            yield return UseAttack(overdrive);
            
        }
    }

    

}
