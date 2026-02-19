using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows.WebCam;

public class BossCombatEntity : CombatEntity
{
    [Header("Boss Settings")]
    [SerializeField] private float aggroRange = 30f;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator bossAnim;
    [SerializeField] private GameObject bossUI;

    [Header("Boss Skills")] 
    [SerializeField] private float tankBusterDamage = 800f;
    [SerializeField] private float tankBusterCastTime = 3f;
    [SerializeField] private float raidWideDamage = 300f;
    [SerializeField] private float raidWideCastTime = 4f;

    private Coroutine rotationCoroutine;
    private Coroutine currentCastRoutine;
    private bool canBeInterrupted = false;
    private bool castInterrupted = false;

    private PlayerCombatEntity currentTarget;
    
    private bool isInCombat = false;

    private void Start()
    {
        currentTarget = FindNearestPlayer();
        agent.SetDestination(currentTarget.gameObject.transform.position);
        OnAutoAttack += () =>  bossAnim.SetTrigger("Auto");
        OnHealthChanged += BossHit;

    }

    void Update()
    {
        HandleAnimations(); 
        if (IsDead) return;
        Vector3 targetPosition = currentTarget.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        
        // Look for players to attack
        if (!isInCombat)
        {
            CheckForPlayer();
        }

        if (!isCasting)
        {
            agent.SetDestination(currentTarget.gameObject.transform.position);
        }
        
        // Continue auto-attacking
        base.Update();
    }
    
    private void CheckForPlayer()
    {
        // Find player
        PlayerCombatEntity player = FindObjectOfType<PlayerCombatEntity>();
        
        if (player == null || player.IsDead)
            return;
        
        // Check distance
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        if (distance <= aggroRange)
        {
            EnterCombat(player);
        }
    }
    
    private void EnterCombat(PlayerCombatEntity player)
    {
        isInCombat = true;
        Debug.Log($"{name} enters combat with {player.name}!");
        StartAutoAttacking(player);
        rotationCoroutine = StartCoroutine(BossRotation());
    }

    IEnumerator BossRotation()
    {
        yield return new WaitForSeconds(5f);
        while (!IsDead)
        {
            
            currentCastRoutine = StartCoroutine(CastAbility("Tank Buster", tankBusterCastTime));
            yield return currentCastRoutine;
            if(!castInterrupted)
                UseTankBuster();
            yield return new WaitForSeconds(8f);

            currentCastRoutine = StartCoroutine(CastAbility("Raidwide", raidWideCastTime, true));
            yield return currentCastRoutine;
            if(!castInterrupted)
                UseRaidWide();
            yield return new WaitForSeconds(8f);
        }
    }

    IEnumerator CastAbility(string abilityName, float castTime, bool interruptable = false)
    {
        agent.SetDestination(transform.position);
        currentTarget = FindNearestPlayer();
        canBeInterrupted = interruptable;
        BeginCasting(abilityName, castTime);
        castInterrupted = false;
        Debug.Log($"Begin casting Boss Ability: {abilityName} ");

        while (currentCastProgress < castTime)
        {
            if (castInterrupted)
            {
                yield break;
            }
            
            currentCastProgress += Time.deltaTime;
            UpdateCastProgress(currentCastProgress);

            yield return null;
        }
        
        CompleteCast();
        agent.SetDestination(currentTarget.transform.position);
    }

    void UseTankBuster()
    {

        PlayerCombatEntity target = currentTarget;
        if (target != null && !target.IsDead)
        {
            Debug.Log("TANK BUSTER!!!");
            target.TakeDamage(tankBusterDamage);
            bossAnim.SetTrigger("Special");
        }
    }

    void UseRaidWide()
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log("RAID WIDE ATTACK!!!");
        foreach (var player in players)
        {
            if (!player.IsDead)
            {
                player.TakeDamage(raidWideDamage);
            }
        }
        
        
    }

    public void InterruptCurrentCast()
    {
        if (!isCasting) return;
        if (!canBeInterrupted)
        {
            Debug.Log($"{name}'s ability cannot be interrupted!");
            return;
        }
        castInterrupted = true;
        InterruptCast();
        agent.SetDestination(currentTarget.transform.position);
        Debug.Log($"{name}'s cast was interrupted!");
        
    }


    PlayerCombatEntity FindNearestPlayer()
    {
        float radius = 40f;

        Collider[] nearby = Physics.OverlapSphere(transform.position, radius);
        PlayerCombatEntity closest = null;

        float minDistance = Mathf.Infinity;

        foreach (var col  in nearby)
        {
            if (col.TryGetComponent(out PlayerCombatEntity target))
            {
                if(target.gameObject == gameObject) continue;

                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = target;
                }
            }
        }
        
        return closest;
    }


    void HandleAnimations()
    {
        bossAnim.SetFloat("Walk", agent.velocity.magnitude);
        bossAnim.SetBool("IsCasting", isCasting);

        if (currentTarget.CurrentHealth <= 0)
        {
            bossAnim.SetBool("Win", true);
        }
    }

    
    void BossHit(float current, float max)
    {
        bossAnim.SetTrigger("Hit");
        if (current <= 0)
        {
            bossAnim.SetTrigger("Died");
        }
        
    }

    protected override void Die()
    {
        if (rotationCoroutine != null)
        {
            InterruptCast();
            StopCoroutine(rotationCoroutine);
        }

        bossUI.SetActive(false);
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 4);
        
    }
    
    
}