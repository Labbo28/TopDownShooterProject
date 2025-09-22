using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Zombie Boss - Boss che lancia Boulder che si frammentano in multiple direzioni
/// </summary>
public class ZombieBossEnemy : EnemyBase
{
    [Header("Boss Settings")]
    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float chargeAttackRange = 8f;

    [Header("Boulder Attack")]
    [SerializeField] private GameObject boulderPrefab;
    [SerializeField] private Transform boulderSpawnPoint;
    [SerializeField] private float boulderCooldown = 8f;
    [SerializeField] private int bouldersPerVolley = 3;
    [SerializeField] private float boulderVolleyDelay = 0.5f;
    [SerializeField] private float boulderDamage = 25f;

    [Header("Charge Attack")]
    [SerializeField] private float chargeCooldown = 12f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float chargeWindupTime = 1f;

    [Header("Boss Phases")]
    [SerializeField] private float phase2HealthThreshold = 0.6f;
    [SerializeField] private float phase3HealthThreshold = 0.3f;

    // Colori personalizzati - FIX per Color.orange
    private static readonly Color Orange = new Color(1f, 0.5f, 0f, 1f); // RGB per arancione
    private static readonly Color DarkOrange = new Color(1f, 0.4f, 0f, 1f); // Arancione scuro

    // Boss phases
    public enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase currentPhase = BossPhase.Phase1;

    // Attack states
    private enum AttackState { Normal, BoulderAttack, ChargeAttack, Cooldown }
    private AttackState currentAttackState = AttackState.Normal;

    // Timing variables
    private float lastBoulderTime = 0f;
    private float lastChargeTime = 0f;
    private bool isPerformingSpecialAttack = false;

    // Charge attack variables
    private Vector3 chargeTarget;
    private bool isCharging = false;

    // Boss stats
    private float baseSpeed;
    private float baseDamage;
    private float baseBoulderCooldown;

    // Events
    public System.Action OnBossPhaseChanged;
    public System.Action<BossPhase> OnBossPhaseTransition;

    protected override void Start()
    {
        base.Start();

        // Boss base stats
        baseSpeed = speed;
        baseDamage = damage;
        baseBoulderCooldown = boulderCooldown;

        // Boss scaling
        if (damage <= 10f) damage = 20f;
        if (attackCooldown <= 1f) attackCooldown = 3f;
        if (speed <= 3f) speed = 3f;

        // Scale health for boss (molto più vita)
        if (healthSystem != null)
        {
            healthSystem.ScaleHealth(6f);
            healthSystem.Heal(500); // Vita extra
        }

        // Setup boulder spawn point se non configurato
        if (boulderSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("BoulderSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.localPosition = new Vector3(0, 2f, 0); // Sopra il boss
            boulderSpawnPoint = spawnPoint.transform;
        }
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

            case AttackState.BoulderAttack:
                StartBoulderAttack();
                break;

            case AttackState.ChargeAttack:
                StartChargeAttack();
                break;

            case AttackState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    /// <summary>
    /// Controlla le transizioni di fase
    /// </summary>
    private void CheckPhaseTransition()
    {
        if (healthSystem == null) return;

        float healthPercentage = healthSystem.GetHealthPercentage();
        BossPhase newPhase = currentPhase;

        if (healthPercentage <= phase3HealthThreshold && currentPhase != BossPhase.Phase3)
        {
            newPhase = BossPhase.Phase3;
        }
        else if (healthPercentage <= phase2HealthThreshold && currentPhase == BossPhase.Phase1)
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

        OnBossPhaseChanged?.Invoke();
        OnBossPhaseTransition?.Invoke(currentPhase);

        switch (currentPhase)
        {
            case BossPhase.Phase2:
                speed = baseSpeed * 1.2f;
                damage = baseDamage * 1.3f;
                boulderCooldown = baseBoulderCooldown * 0.8f;
                bouldersPerVolley = 4; // Più boulder per volley
                StartCoroutine(PhaseTransitionEffect(Orange)); // FIX: Usa Orange personalizzato
                break;

            case BossPhase.Phase3:
                speed = baseSpeed * 1.5f;
                damage = baseDamage * 1.6f;
                boulderCooldown = baseBoulderCooldown * 0.6f;
                bouldersPerVolley = 5; // Ancora più boulder
                boulderVolleyDelay = 0.3f; // Più veloce
                StartCoroutine(PhaseTransitionEffect(Color.red));
                break;
        }
    }

    /// <summary>
    /// Determina quale attacco usare
    /// </summary>
    private AttackState DetermineNextAttack(float distanceToPlayer)
    {
        // Boulder attack - attacco principale a distanza
        if (Time.time >= lastBoulderTime + boulderCooldown &&
            distanceToPlayer <= attackRange && distanceToPlayer > meleeAttackRange)
        {
            return AttackState.BoulderAttack;
        }

        // Charge attack - quando il player è a media distanza
        if (Time.time >= lastChargeTime + chargeCooldown &&
            distanceToPlayer <= chargeAttackRange &&
            distanceToPlayer > meleeAttackRange * 2 &&
            currentPhase >= BossPhase.Phase2)
        {
            return AttackState.ChargeAttack;
        }

        return AttackState.Normal;
    }

    /// <summary>
    /// Attacco normale melee
    /// </summary>
    private void HandleNormalAttack(float distanceToPlayer)
    {
        if (distanceToPlayer <= meleeAttackRange)
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
            // Muovi verso il player
            Move(player.position, speed);
        }
    }

    /// <summary>
    /// Inizia l'attacco Boulder
    /// </summary>
    private void StartBoulderAttack()
    {
        if (isPerformingSpecialAttack) return;

        isPerformingSpecialAttack = true;
        lastBoulderTime = Time.time;

        StartCoroutine(PerformBoulderAttack());
    }

    /// <summary>
    /// Esegue l'attacco Boulder
    /// </summary>
    private IEnumerator PerformBoulderAttack()
    {
        // Stop movement durante l'attacco
        StopMoving();

        // Windup animation
        StartCoroutine(BoulderWindupEffect());
        yield return new WaitForSeconds(1.5f);

        // Lancia i boulder in volley
        for (int i = 0; i < bouldersPerVolley; i++)
        {
            LaunchBoulder();
            yield return new WaitForSeconds(boulderVolleyDelay);
        }

        // Cooldown
        yield return new WaitForSeconds(1f);
        isPerformingSpecialAttack = false;
    }

    /// <summary>
    /// Lancia un singolo boulder
    /// </summary>
    private void LaunchBoulder()
    {
        if (boulderPrefab == null || player == null) return;

        // Calcola target con un po' di spread
        Vector3 targetPosition = player.position;

        // Aggiungi variazione casuale per rendere più difficile schivare
        float spread = 2f;
        if (currentPhase >= BossPhase.Phase2) spread = 1f; // Più preciso nelle fasi avanzate

        targetPosition.x += Random.Range(-spread, spread);
        targetPosition.y += Random.Range(-spread, spread);

        // Spawn boulder
        Vector3 spawnPos = boulderSpawnPoint != null ? boulderSpawnPoint.position : transform.position + Vector3.up * 2f;
        GameObject boulder = Instantiate(boulderPrefab, spawnPos, Quaternion.identity);

        // Configure boulder
        BoulderProjectile boulderScript = boulder.GetComponent<BoulderProjectile>();
        if (boulderScript != null)
        {
            boulderScript.SetTarget(targetPosition);
            boulderScript.SetBoulderParameters(boulderDamage, 8f, 6 + (int)currentPhase * 2);
        }

        // Sound effect
        StartCoroutine(BoulderLaunchEffect());
    }

    /// <summary>
    /// Inizia attacco di carica
    /// </summary>
    private void StartChargeAttack()
    {
        if (isPerformingSpecialAttack) return;

        isPerformingSpecialAttack = true;
        lastChargeTime = Time.time;
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
        yield return new WaitForSeconds(chargeWindupTime);

        // Carica
        isCharging = true;
        float chargeTimer = 0f;
        Vector3 startPos = transform.position;

        while (chargeTimer < chargeDuration && isCharging)
        {
            // Movimento verso il target
            transform.position = Vector3.MoveTowards(transform.position, chargeTarget, chargeSpeed * Time.deltaTime);

            // Controlla collisione con player
            if (DistanceToPlayer() <= meleeAttackRange)
            {
                DamagePlayer(damage * 2f); // Danno doppio per la carica
                StartCoroutine(ChargeImpactEffect());
                break;
            }

            chargeTimer += Time.deltaTime;
            yield return null;
        }

        isCharging = false;

        // Stun breve dopo la carica
        yield return new WaitForSeconds(1.5f);
        isPerformingSpecialAttack = false;
    }

    /// <summary>
    /// Gestisce il cooldown
    /// </summary>
    private void HandleCooldown()
    {
        // Movimento lento durante il cooldown
        if (DistanceToPlayer() > meleeAttackRange * 1.5f)
        {
            Move(player.position, speed * 0.7f);
        }
    }

    #region Visual Effects

    private IEnumerator PhaseTransitionEffect(Color phaseColor)
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 6; i++)
            {
                spriteRenderer.color = phaseColor;
                yield return new WaitForSeconds(0.25f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    private IEnumerator BossAttackFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator BoulderWindupEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            for (int i = 0; i < 15; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private IEnumerator BoulderLaunchEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Orange; // FIX: Usa Orange personalizzato
            yield return new WaitForSeconds(0.1f);
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
                spriteRenderer.color = Color.cyan;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
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

    #endregion

    public override EnemyType GetEnemyType()
    {
        return EnemyType.ZombieBoss; // Dovrai aggiungere questo all'enum EnemyType
    }

    /// <summary>
    /// Get current boss phase
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

    protected override void ResetStateAfterRepositioning()
    {
        base.ResetStateAfterRepositioning();
        isPerformingSpecialAttack = false;
        isCharging = false;
        currentAttackState = AttackState.Normal;
    }

    // Debug visualization
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Boulder attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Melee attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        // Charge attack range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chargeAttackRange);

        // Boulder spawn point
        if (boulderSpawnPoint != null)
        {
            Gizmos.color = Orange; // FIX: Usa Orange personalizzato
            Gizmos.DrawSphere(boulderSpawnPoint.position, 0.3f);
        }

        // Charge target
        if (isCharging && chargeTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, chargeTarget);
            Gizmos.DrawSphere(chargeTarget, 0.5f);
        }
    }
}