using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class AIPlayerController : MonoBehaviour
{
    [SerializeField] private PlayerCombatEntity player;
    [SerializeField] private float thinkTime = 2.5f; // uses an ability every X seconds
    [SerializeField] private float followDistance = 8f; // stays away from boss this distance

    private BossCombatEntity boss;
    private NavMeshAgent agent;
    
    
    
    private float thinkTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        boss = FindObjectOfType<BossCombatEntity>();
        thinkTimer = thinkTime;
        agent.SetDestination(boss.transform.position);
        agent.stoppingDistance = followDistance;
    }

    private void Update()
    {
        if (player.IsDead) return;
        
        if (boss == null || boss.IsDead) return;
        MaintainPosition(boss);
        
        thinkTimer -= Time.deltaTime;
        if (thinkTimer <= 0f)
        {
            agent.SetDestination(boss.transform.position);
            ThinkAboutAbilities(boss);
            thinkTimer = thinkTime;
        }
        
    }
    
    private void MaintainPosition(BossCombatEntity boss)
    {
        if (boss.CurrentCastName() == "Overdrive")
        {
            agent.stoppingDistance = 15;
        }
        else
        {
            agent.stoppingDistance = followDistance;
        }
    }


    void ThinkAboutAbilities(BossCombatEntity boss)
    {
        if (!player.CanAct()) return;
        
        
        UseRandomDamageAbility(boss);
    }
    
    private void UseRandomDamageAbility(BossCombatEntity boss)
    {
        // Find damage abilities (index 0, 3, etc.)
        int[] damageAbilityIndices = { 0, 1,2,3 }; // Fireball, Quick Shot
        
        int randomIndex = damageAbilityIndices[Random.Range(0, damageAbilityIndices.Length)];
        
        if (randomIndex < player.Spells.Count)
        {
            player.TryCastSpell(player.Spells[randomIndex]);
        }
    }
}
