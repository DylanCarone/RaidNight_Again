using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class BossCombatEntity : CombatEntity
{
    [Header("Boss Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected Animator bossAnim;
    [SerializeField] private GameObject bossUI;
    [SerializeField] protected LayerMask obstacleMask;


    
    
    protected Coroutine rotationCoroutine;
    protected Coroutine currentCastRoutine;
    private bool canBeInterrupted = false;
    protected bool castInterrupted = false;
    private bool lookingAtPlayer = true;

    
    private bool isInCombat = false;

    private int attackID;

    private void Start()
    {
        currentTarget = FindNearestPlayer();
        agent.SetDestination(currentTarget.gameObject.transform.position);
        OnAutoAttack += () =>
        {
            if (bossUI == null) return;
            bossAnim?.SetTrigger("Auto");
            ChooseRandomAttackAnimation();
        };
        OnHealthChanged += BossHit;
        
        
    }

    protected void Update()
    {
        HandleAnimations(); 
        if (IsDead) return;

        if (currentTarget.IsDead)
        {
            currentTarget = GetPriorityTarget();
        }
        if (lookingAtPlayer)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        if (currentTarget == null)
        {
            StopAllCoroutines();
            agent.isStopped = true;
            return;
        }
        
        // Look for players to attack
        if (!isInCombat)
            CheckForPlayer();
        

        if (!isCasting && !currentTarget.IsDead)
            agent.SetDestination(currentTarget.gameObject.transform.position);
        
        
        // Continue auto-attacking
        base.Update();
    }

    protected override void UpdateAutoAttack()
    {
        currentTarget = GetPriorityTarget();
        base.UpdateAutoAttack();
    }
    
    private void CheckForPlayer()
    {
        // Find player
        PlayerCombatEntity player = GetPriorityTarget() as PlayerCombatEntity;
        
        if (player == null || player.IsDead)
            return;
        
        // Check distance
        EnterCombat(player);
        
    }
    
    private void EnterCombat(PlayerCombatEntity player)
    {
        isInCombat = true;
        //Debug.Log($"{name} enters combat with {player.name}!");
        StartAutoAttacking(player);
        rotationCoroutine = StartCoroutine(BossRotation());
    }




    protected abstract IEnumerator BossRotation();
    
    protected IEnumerator UseAttack(BossAbility ability)
    {
        GameObject telegraph = (ability is BossAbilityTelegraph t) ? t.telegraphVisual : null;
        
        currentCastRoutine = StartCoroutine(CastAbility(ability, telegraph));
        yield return currentCastRoutine;

        if (!castInterrupted)
        {
            switch (ability.attackType)
            {
                case AttackType.SingleTarget:
                    UseSingleTarget(ability);
                    break;
                case AttackType.RaidWide:
                    UseRaidWide(ability);
                    break;
                case AttackType.LineAOE:
                    UseLineAOE(ability);
                    break;
                case AttackType.AreaAOE:
                    UseBigAOEAttack(ability);
                    break;
            }

        }

    }


    private IEnumerator CastAbility(BossAbility ability, GameObject telegraph = null)
    {
        agent.isStopped = true;
        
        canBeInterrupted = ability.isInterruptible;
        BeginCasting(ability.abilityName, ability.castTime);
        castInterrupted = false;

        if (telegraph != null)
        {
            telegraph.SetActive(true);

            if (ability.attackType == AttackType.AreaAOE)
            {
                Transform aoeVisual = telegraph.transform;
                aoeVisual.localScale = new Vector3(ability.range * 2, 0.2f, ability.range * 2);
            }
        }

        
        while (currentCastProgress < ability.castTime)
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
        if (telegraph != null)
            telegraph.SetActive(false);
        
        agent.isStopped = false;
    }

    private void UseSingleTarget(BossAbility ability)
    {
        CombatEntity target = currentTarget;
        if (target != null && !target.IsDead)
        {
            target.TakeDamage(ability.damage);
            bossAnim?.SetTrigger(ability.animation.ToString());
        }
    }

    private void UseRaidWide(BossAbility ability)
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (var player in players)
        {
            if (!player.IsDead)
                player.TakeDamage(ability.damage);
            
        }
        bossAnim?.SetTrigger(ability.animation.ToString());
    }

    private void UseLineAOE(BossAbility ability)
    {
        bossAnim?.SetTrigger(ability.animation.ToString());
        Vector3 center = transform.position + transform.forward * (ability.range / 2f);
        Vector3 halfExtents = new Vector3(ability.range / 2f, 0.25f, ability.range / 2f);
        
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, transform.rotation);

        
        foreach (var collider in hitColliders)
        {
            PlayerCombatEntity player = collider.GetComponent<PlayerCombatEntity>();
            if (player == null || player.IsDead)
                continue;
            

            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            Vector3 rayStart = transform.position + Vector3.up * 1.2f;

            RaycastHit hit;
            if (!Physics.Raycast(rayStart, directionToPlayer, out hit, distanceToPlayer, obstacleMask))
                player.TakeDamage(ability.damage);
        }
    }
    private void UseBigAOEAttack(BossAbility ability)
    {
        if (ability is BossAbilityTelegraph t && t.telegraphVisual != null)
        {
            Transform aoeVisual = t.telegraphVisual.transform;
            aoeVisual.localScale = new Vector3(ability.range * 2, 0.2f, ability.range * 2);
        }
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ability.range);
        bossAnim?.SetTrigger(ability.animation.ToString());

        foreach (var colliders in hitColliders)
        {
            PlayerCombatEntity player = colliders.GetComponent<PlayerCombatEntity>();
            if (player == null || player.IsDead) continue;

            if (ability is BossAbilityTelegraph a && a.lineOfSight)
            {

                Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                RaycastHit hit;
                if (!Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleMask))
                    player.TakeDamage(ability.damage);
            }
            else
            {
                player.TakeDamage(ability.damage);
            }
        }

    }
    
    

    public void InterruptCurrentCast()
    {
        if (!isCasting) return;
        if (!canBeInterrupted)
            return;
        
        castInterrupted = true;
        InterruptCast();
        agent.SetDestination(currentTarget.transform.position);
    }


    CombatEntity FindNearestPlayer()
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
        
        return closest as CombatEntity;
    }


    void HandleAnimations()
    {
        if (bossAnim == null) return;
        bossAnim?.SetFloat("Walk", agent.velocity.magnitude);
        bossAnim?.SetBool("IsCasting", isCasting);

        if (currentTarget.IsDead)
        {
            bossAnim?.SetBool("Win", true);
        }
    }

    void ChooseRandomAttackAnimation()
    {
        if(bossAnim == null) return;
        attackID = Random.Range(0, 2);
        bossAnim?.SetFloat("AttackID", attackID);
    }

    
    void BossHit(float current, float max)
    {
        if(bossAnim == null) return;
        bossAnim?.SetTrigger("Hit");
        if (current <= 0)
        {
            bossAnim?.SetTrigger("Died");
        }
        
    }

    protected override void Die()
    {
        if (rotationCoroutine != null)
        {
            InterruptCast();
            StopCoroutine(rotationCoroutine);
        }
        agent.SetDestination(transform.position);

        bossUI.SetActive(false);
        base.Die();
    }

    private void SwitchTarget()
    {
        currentTarget = GetPriorityTarget();
    }

    private CombatEntity GetPriorityTarget()
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        PlayerCombatEntity prioTarget = null;
        int highestPriority = -1;
        float closestDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            if (player == null || player.IsDead) continue;

            int currentPriority = 0;
            switch (player.Role)
            {
                case PlayerRole.Tank: currentPriority = 3; break; 
                case PlayerRole.Melee: currentPriority = 2; break; 
                case PlayerRole.Ranged: currentPriority = 1; break; 
                case PlayerRole.Healer: currentPriority = 0; break; 
            }

            if (currentPriority > highestPriority)
            {
                highestPriority = currentPriority;
                prioTarget = player;
                closestDistance = Vector3.Distance(transform.position, player.transform.position);
            }
            else if (currentPriority == highestPriority)
            {
                    prioTarget = player;
            }
        }

        return prioTarget;
    }

    
    
}






















