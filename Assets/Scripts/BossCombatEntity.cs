using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows.WebCam;
using Random = UnityEngine.Random;

public class BossCombatEntity : CombatEntity
{
    [Header("Boss Settings")]
    [SerializeField] private float aggroRange = 30f;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator bossAnim;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Boss Skills")] 
    [Header("Slam")]
    [SerializeField] private float tankBusterDamage = 800f;
    [SerializeField] private float tankBusterCastTime = 2.5f;
    [Header("Unavoidable")]
    [SerializeField] private float raidWideDamage = 300f;
    [SerializeField] private float raidWideCastTime = 2f;
    [Header("Big AOE")] 
    [SerializeField] private GameObject aoeIndicator;
    [SerializeField] private float aoeDamage = 1200f;
    [SerializeField] private float aoeRadius = 20f;
    [SerializeField] private float aoeCastTime = 7f;
    
    
    [Header("Interruptible")]
    [SerializeField] private float interruptibleDamage = 1200f;
    [SerializeField] private float interruptibleCastTime = 3f;
    [Header("Line AOE")]
    [SerializeField] private GameObject lineIndicator;
    [SerializeField] private float lineDamage = 800f;
    [SerializeField] private float lineCastTime = 4f;
    [SerializeField] private float lineRange = 5f;
    [SerializeField] private float lineWidth = 5f;
    
    [Header("Circle AOE")]
    [SerializeField] private float circleDamage = 900f;
    [SerializeField] private float circleCastTime = 4f;
    
    
    private Coroutine rotationCoroutine;
    private Coroutine currentCastRoutine;
    private bool canBeInterrupted = false;
    private bool castInterrupted = false;
    private bool lookingAtPlayer = true;

    private PlayerCombatEntity currentTarget;
    
    private bool isInCombat = false;

    private int attackID;

    private void Start()
    {
        currentTarget = FindNearestPlayer();
        agent.SetDestination(currentTarget.gameObject.transform.position);
        OnAutoAttack += () =>
        {
            bossAnim.SetTrigger("Auto");
            ChooseRandomAttackAnimation();
        };
        OnHealthChanged += BossHit;

        aoeIndicator.transform.localScale = new Vector3(aoeRadius * 2f, 0.2f, aoeRadius * 2f);
    }

    void Update()
    {
        HandleAnimations(); 
        if (IsDead) return;
        if (lookingAtPlayer)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        if (currentTarget.IsDead)
        {
            StopAllCoroutines();
            agent.SetDestination(transform.position);
        }

        
        
        // Look for players to attack
        if (!isInCombat)
        {
            CheckForPlayer();
        }

        if (!isCasting && !currentTarget.IsDead )
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
        
        yield return new WaitForSeconds(0.2f);
        // remove starting wait, just go into AOE from the gate
        // AOE damage move to start
        currentCastRoutine = StartCoroutine(CastAbility("Blast", raidWideCastTime));
        yield return currentCastRoutine;
        if(!castInterrupted)
            UseRaidWide();
        // Wait few seconds
        yield return new WaitForSeconds(4f);
        
        while (!IsDead)
        {
            // Slam into LOS AOE right away
            currentCastRoutine = StartCoroutine(CastAbility("Crush", tankBusterCastTime));
            yield return currentCastRoutine;
            if(!castInterrupted)
                UseSlamAttack();
            
            yield return new WaitForSeconds(0.5f);
            
            currentCastRoutine = StartCoroutine(CastTelegraph("Eruption", aoeCastTime, aoeIndicator));
            yield return currentCastRoutine;
            UseBigAOEAttack();
            
            // Wait
            yield return new WaitForSeconds(8f);
            
            // Big Interruptible attack
            currentCastRoutine = StartCoroutine(CastAbility("~~Overcharge~~", interruptibleCastTime, true));
            yield return currentCastRoutine;
            if(!castInterrupted)
                UseInterrupitble();
            // Wait
            yield return new WaitForSeconds(4f);
            
            // Slam into Line AOE
            currentCastRoutine = StartCoroutine(CastAbility("Crush", tankBusterCastTime));
            yield return currentCastRoutine;
            if(!castInterrupted)
                UseSlamAttack();
            
            yield return new WaitForSeconds(0.5f);
            
            currentCastRoutine = StartCoroutine(CastTelegraph("Shockwave", lineCastTime, lineIndicator));
            yield return currentCastRoutine;
            UseLineAOEAttack();
            
            // wait
            yield return new WaitForSeconds(8f);

            // Circle on player gotta dodge
            // Wait

            // Big Interruptible attack
            // Wait

            // Loop




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

    IEnumerator CastTelegraph(string abilityName, float castTime, GameObject telegraph,
        bool interruptable = false)
    {
        //lookingAtPlayer = false;
        agent.SetDestination(transform.position);
        canBeInterrupted = interruptable;
        BeginCasting(abilityName, castTime);
        castInterrupted = false;

        telegraph.SetActive(true);

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
        telegraph.SetActive(false);
        lookingAtPlayer = true;
        agent.SetDestination(currentTarget.transform.position);
    }

    void UseSlamAttack()
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
        bossAnim.SetTrigger("Special_2");
    }
    void UseInterrupitble()
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log("RAID WIDE ATTACK!!!");
        foreach (var player in players)
        {
            if (!player.IsDead)
            {
                player.TakeDamage(interruptibleDamage);
            }
        }
        bossAnim.SetTrigger("Special_2");
    }

    void UseBigAOEAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius);
        bossAnim.SetTrigger("Special_2");

        foreach (var colliders in hitColliders)
        {
            PlayerCombatEntity player = colliders.GetComponent<PlayerCombatEntity>();
            if (player == null || player.IsDead) continue;

            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleMask))
            {
                Debug.Log($"{player.name} is SAFE behind {hit.collider.name}");
            }
            else
            {
                Debug.Log($"{player.name} was not safe and got hit...");
                player.TakeDamage(aoeDamage);
                
            }
        }
    }
    void UseLineAOEAttack()
    {
        Debug.Log("Using Shockwave.");
        bossAnim.SetTrigger("Special");
        Vector3 center = transform.position + transform.forward * (lineRange / 2f);
        Vector3 halfExtents = new Vector3(lineWidth / 2f, 0.25f, lineRange / 2f);
        
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, transform.rotation);

        
        foreach (var collider in hitColliders)
        {
            Debug.Log(collider.name);
            PlayerCombatEntity player = collider.GetComponent<PlayerCombatEntity>();
            if (player == null || player.IsDead)
            {
                continue;
            }

            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            Vector3 rayStart = transform.position + Vector3.up * 1.2f;

            RaycastHit hit;
            if (Physics.Raycast(rayStart, directionToPlayer, out hit, distanceToPlayer, obstacleMask))
            {
                Debug.Log($"{player.name} is SAFE behind {hit.collider.name}");
            }
            else
            {
                Debug.Log($"{player.name} was not safe and got hit...");
                player.TakeDamage(lineDamage);
                
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

    void ChooseRandomAttackAnimation()
    {
        attackID = Random.Range(0, 2);
        bossAnim.SetFloat("AttackID", attackID);
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

    void OnDrawGizmos()
    {
        // Draw the line AoE box
        Vector3 center = transform.position + transform.forward * (lineRange / 2f);
        Vector3 size = new Vector3(lineWidth, 0.5f, lineRange); // Full size, not half
    
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity; // Reset matrix
    
        // Draw direction arrow
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * lineRange);
    }
    
    
}