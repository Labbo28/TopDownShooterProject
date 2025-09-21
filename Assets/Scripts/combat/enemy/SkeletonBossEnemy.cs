using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Skeleton Boss - Boss finale con multiple fasi, attacchi speciali e capacità di evocazione
/// </summary>
public class SkeletonBossEnemy : EnemyBase
{
    [Header("Boss Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chargeAttackRange = 8f;
    [SerializeField] private float areaAttackRange = 4f;
    [SerializeField] private float summonRange = 10f;
    
    [Header("Boss Abilities")]
    [SerializeField] private GameObject minionPrefab; // Prefab dei nemici da evocare
    [SerializeField] private int maxMinions = 20;
    [SerializeField] private float summonCooldown = 15f;
    [SerializeField] private float chargeAttackCooldown = 8f;
    [SerializeField] private float areaAttackCooldown = 12f;
    [SerializeField] private float minionsHealth=120f;
    
   
    // Boss phases
    public enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase currentPhase = BossPhase.Phase1;
    
    // Attack states
    private enum AttackState { Normal, Charging, AreaAttack, Summoning, Cooldown }
    private AttackState currentAttackState = AttackState.Normal;
    
    // Timing variables
    private float lastSummonTime = 0f;
    private float lastChargeAttackTime = 0f;
    private float lastAreaAttackTime = 0f;
    private float phaseTransitionTime = 0f;
    
    // Attack targets and positions
    private Vector3 chargeTarget;
    private bool isPerformingSpecialAttack = false;
    private List<GameObject> summonedMinions = new List<GameObject>();
    
    // Boss stats per phase
    private float baseSpeed;
    private float baseDamage;
    private bool hasUsedPhase2Ability = false;
    private bool hasUsedPhase3Ability = false;

    // Boss wave integration
    public System.Action OnBossPhaseChanged;
    public System.Action<BossPhase> OnBossPhaseTransition;

    protected override void Start()
    {
        base.Start();
        
        // Boss base stats - molto più forti del normale
        baseSpeed = speed;
        baseDamage = damage;
        
    }

    protected override void HandleBehavior()
    {
        if (isPerformingSpecialAttack) return;
        
        CheckPhaseTransition();
        
        float distanceToPlayer = DistanceToPlayer();
        
        // Decide quale attacco usare
        AttackState nextAttack = DetermineNextAttack(distanceToPlayer);
        
        switch (nextAttack)
        {
            case AttackState.Normal:
                HandleNormalAttack(distanceToPlayer);
                break;
                
            case AttackState.Charging:
                StartChargeAttack();
                break;
                
            case AttackState.AreaAttack:
                StartAreaAttack();
                break;
                
            case AttackState.Summoning:
                StartSummonAttack();
                break;
                
            case AttackState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    /// <summary>
    /// Controlla se è necessario cambiare fase
    /// </summary>
    private void CheckPhaseTransition()
    {
        if (healthSystem == null) return;
        
        float healthPercentage = healthSystem.GetHealthPercentage();
        
        BossPhase newPhase = currentPhase;
        
        if (healthPercentage <= 0.3f && currentPhase != BossPhase.Phase3)
        {
            newPhase = BossPhase.Phase3;
        }
        else if (healthPercentage <= 0.6f && currentPhase == BossPhase.Phase1)
        {
            newPhase = BossPhase.Phase2;
        }
        
        if (newPhase != currentPhase)
        {
            TransitionToPhase(newPhase);
        }
    }

    /// <summary>
    /// Transizione a una nuova fase
    /// </summary>
    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
        phaseTransitionTime = Time.time;

        // Notify listeners about phase transition
        OnBossPhaseChanged?.Invoke();
        OnBossPhaseTransition?.Invoke(currentPhase);

        switch (currentPhase)
        {
            case BossPhase.Phase2:
                speed = baseSpeed * 1.3f; // Diventa più veloce
                damage = baseDamage * 1.2f; // Più danno
                summonCooldown *= 0.8f; // Evoca più spesso
                StartCoroutine(PhaseTransitionEffect(Color.yellow));
                break;

            case BossPhase.Phase3:
                speed = baseSpeed * 1.6f; // Ancora più veloce
                damage = baseDamage * 1.5f; // Danno massimo
                attackCooldown *= 0.7f; // Attacca più spesso
                summonCooldown *= 0.6f; // Evoca molto più spesso
                maxMinions = 30; // Più minions nella fase finale
                StartCoroutine(PhaseTransitionEffect(Color.red));
                break;
        }
    }

    /// <summary>
    /// Determina quale attacco usare
    /// </summary>
    private AttackState DetermineNextAttack(float distanceToPlayer)
    {
        // Controlla se può evocare minions
        if (Time.time >= lastSummonTime + summonCooldown && 
            summonedMinions.Count < maxMinions &&
            distanceToPlayer <= summonRange)
        {
            return AttackState.Summoning;
        }
        
        // Controlla attacco ad area (solo se abbastanza vicino)
        if (Time.time >= lastAreaAttackTime + areaAttackCooldown && 
            distanceToPlayer <= areaAttackRange &&
            currentPhase >= BossPhase.Phase2)
        {
            return AttackState.AreaAttack;
        }
        
        // Controlla attacco di carica (solo se abbastanza lontano)
        if (Time.time >= lastChargeAttackTime + chargeAttackCooldown && 
            distanceToPlayer <= chargeAttackRange && 
            distanceToPlayer > attackRange * 2)
        {
            return AttackState.Charging;
        }
        
        // Attacco normale
        return AttackState.Normal;
    }

    /// <summary>
    /// Attacco normale melee
    /// </summary>
    private void HandleNormalAttack(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            Attack();

            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage);
                isAttacking = false;
                StartCoroutine(BossAttackFlash());
            }
        }
        else
        {
            Move(player.position, speed);
        }
    }

    /// <summary>
    /// Inizia attacco di carica
    /// </summary>
    private void StartChargeAttack()
    {
        if (isPerformingSpecialAttack) return;
        
        isPerformingSpecialAttack = true;
        lastChargeAttackTime = Time.time;
        chargeTarget = player.position;
        
        StartCoroutine(PerformChargeAttack());
    }

    /// <summary>
    /// Esegue l'attacco di carica
    /// </summary>
    private IEnumerator PerformChargeAttack()
    {
        // Windup
        StartCoroutine(ChargeWindupEffect());
        yield return new WaitForSeconds(1f);
        
        // Carica veloce verso il target
        float chargeSpeed = speed * 3f;
        float chargeDuration = 1.5f;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsedTime < chargeDuration)
        {
            transform.position = Vector3.Lerp(startPos, chargeTarget, elapsedTime / chargeDuration);
            
            // Controlla collisione con player durante la carica
            if (DistanceToPlayer() <= attackRange)
            {
                DamagePlayer(damage * 2f); // Danno doppio per la carica
                StartCoroutine(ChargeImpactEffect());
                break;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Cooldown dopo la carica
        yield return new WaitForSeconds(1f);
        isPerformingSpecialAttack = false;
    }

    /// <summary>
    /// Inizia attacco ad area
    /// </summary>
    private void StartAreaAttack()
    {
        if (isPerformingSpecialAttack) return;
        
        isPerformingSpecialAttack = true;
        lastAreaAttackTime = Time.time;
        
        StartCoroutine(PerformAreaAttack());
    }

    /// <summary>
    /// Esegue attacco ad area
    /// </summary>
    private IEnumerator PerformAreaAttack()
    {
        // Preparazione
        StartCoroutine(AreaAttackWindupEffect());
        yield return new WaitForSeconds(1.5f);
        
        // Attacco ad area
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, areaAttackRange);
        
        foreach (Collider2D col in enemiesInRange)
        {
            if (col.CompareTag("Player"))
            {
                DamagePlayer(damage * 1.5f);
                break;
            }
        }
        
        StartCoroutine(AreaAttackExplosionEffect());
        
        yield return new WaitForSeconds(1f);
        isPerformingSpecialAttack = false;
    }

    /// <summary>
    /// Inizia evocazione di minions
    /// </summary>
    private void StartSummonAttack()
    {
        if (isPerformingSpecialAttack) return;
        
        isPerformingSpecialAttack = true;
        lastSummonTime = Time.time;
        
        StartCoroutine(PerformSummonAttack());
    }

    /// <summary>
    /// Esegue l'evocazione
    /// </summary>
    private IEnumerator PerformSummonAttack()
    {
        // Animazione di evocazione
        StartCoroutine(SummonWindupEffect());
        yield return new WaitForSeconds(2f);
        
        // Evoca minions
        int minionsToSummon = maxMinions;
        float angleStep = 360f / minionsToSummon;
        float radius = 6f;

        for (int i = 0; i < minionsToSummon; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad; // converti in radianti
            float x = player.position.x + Mathf.Cos(angle) * radius;
            float y = player.position.y + Mathf.Sin(angle) * radius;

            Vector2 spawnPos = new Vector2(x, y);
            GameObject skeleton = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
            modifyMinion(skeleton);
        }
        
        // Pulizia dei minions morti
        summonedMinions.RemoveAll(minion => minion == null);
        
        yield return new WaitForSeconds(1f);
        isPerformingSpecialAttack = false;
    }

    private void modifyMinion(GameObject skeleton)
    {
        skeleton.GetComponent<SpriteRenderer>().color = Color.magenta;
        skeleton.GetComponent<HealthSystem>().SetMaxHealth(minionsHealth);
    }
    /// <summary>
    /// Gestisce il cooldown
    /// </summary>
    private void HandleCooldown()
    {
        // Movimento lento durante il cooldown
        if (DistanceToPlayer() > attackRange * 1.5f)
        {
            Move(player.position, speed * 0.5f);
        }
    }

    #region Visual Effects

    private IEnumerator PhaseTransitionEffect(Color phaseColor)
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = phaseColor;
                yield return new WaitForSeconds(0.2f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator BossAttackFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ChargeWindupEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 10; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private IEnumerator ChargeImpactEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator AreaAttackWindupEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 15; i++)
            {
                spriteRenderer.color = Color.magenta;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator AreaAttackExplosionEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator SummonWindupEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 20; i++)
            {
                spriteRenderer.color = Color.green;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator SummonEffect(Vector3 position)
    {
        // Qui potresti istanziare un effetto particellare
        yield return new WaitForSeconds(0.1f);
    }

    #endregion

    public override EnemyType GetEnemyType()
    {
        return EnemyType.SkeletonBoss;
    }

    /// <summary>
    /// Get current boss phase for external systems
    /// </summary>
    public BossPhase GetCurrentPhase()
    {
        return currentPhase;
    }

    /// <summary>
    /// Get boss health percentage
    /// </summary>
    public float GetHealthPercentage()
    {
        return healthSystem?.GetHealthPercentage() ?? 0f;
    }

    /// <summary>
    /// Override per comportamento specifico dopo riposizionamento
    /// </summary>
    protected override void ResetStateAfterRepositioning()
    {
        base.ResetStateAfterRepositioning();
        isPerformingSpecialAttack = false;
        currentAttackState = AttackState.Normal;
    }

    // Visualizzazione debug
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Disegna i vari raggi di attacco
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeAttackRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, areaAttackRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, summonRange);
        
        // Mostra il target della carica se sta caricando
        if (currentAttackState == AttackState.Charging)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, chargeTarget);
            Gizmos.DrawSphere(chargeTarget, 0.5f);
        }
    }


}