using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBoss : BossCombatEntity
{
    [Header("Ability Data")]
    [SerializeField] private BossAbility raidWide;
    [SerializeField] private BossAbility slam;
    [SerializeField] private BossAbility blastoff;
    [SerializeField] private BossAbility balls;
    [SerializeField] private BossAbilityTelegraph shockWave;
    [SerializeField] private BossAbilityTelegraph overdrive;


    protected override IEnumerator BossRotation()
    {
        yield return new WaitForSeconds(2f);
        while (!IsDead)
        {
            yield return UseAttack(balls);
            yield return new WaitForSeconds(4);
            yield return UseAttack(slam);
            yield return new WaitForSeconds(60);
            


        }
    }


    public List<GameObject> tests = new List<GameObject>();
    protected override void UseRoleTargeted(BossAbility ability)
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.Role == PlayerRole.Ranged || player.Role == PlayerRole.Healer)
            {
                Vector3 playerPos = new Vector3(player.transform.position.x, 0.1f, player.transform.position.z);
                var ball = Instantiate(balls.spawnPrefab, playerPos, player.transform.rotation);
                tests.Add(ball);
                Debug.Log($"Dropping Balls on {player.name} - {player.Role}");
            } 
        }
    }
    

    

}
