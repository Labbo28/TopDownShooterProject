using UnityEngine;
using System.Collections;

/// <summary>
/// Zombie Veloce - Più agile e aggressivo del zombie normale, attacca in gruppi
/// </summary>
public class ZombieFastEnemy : EnemyBase
{
    [Header("Fast Zombie Settings")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float chargeSpeed = 8f; // Velocità durante la carica
    [SerializeField] private float chargeDistance = 5f; // Distanza per iniziare la carica
    [SerializeField] private float chargeCooldown = 3f; // Cooldown della carica
    
    private bool isCharging = false;
    private float lastChargeTime = 0f;
    private Vector3 chargeTarget;
    private float baseSpeed;

    protected override void Start()
    {
        base.Start();
        baseSpeed = speed;
        
        // Fast Zombie settings - più veloci ma meno resistenti// Più veloce del zombie normale
    }

    protected override void HandleBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();

        // Se sta caricando, continua la carica
        if (isCharging)
        {
            HandleCharge();
            return;
        }

        // Se il player è nel raggio di attacco
        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            Attack();

            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage);
                isAttacking = false;
                
                // Effetto di attacco rapido
                StartCoroutine(FastAttackFlash());
            }
        }
        // Se il player è nella distanza di carica e il cooldown è finito
        else if (distanceToPlayer <= chargeDistance && 
                 distanceToPlayer > attackRange && 
                 Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCharge();
        }
        // Altrimenti insegue normalmente - always pursue player
        else
        {
            Move(player.position, speed);
        }
    }

    /// <summary>
    /// Inizia la carica verso il player
    /// </summary>
    private void StartCharge()
    {
        isCharging = true;
        lastChargeTime = Time.time;
        chargeTarget = player.position;
        
        // Effetto visivo di preparazione alla carica
        StartCoroutine(ChargeWindup());
    }

    /// <summary>
    /// Gestisce il movimento durante la carica
    /// </summary>
    private void HandleCharge()
    {
        float distanceToTarget = Vector3.Distance(transform.position, chargeTarget);
        
        if (distanceToTarget <= 0.5f)
        {
            // Fine della carica
            EndCharge();
            return;
        }
        
        // Muovi verso il target della carica
        Move(chargeTarget, chargeSpeed);
        
        // Se si avvicina troppo al player durante la carica, prova ad attaccarlo
        if (DistanceToPlayer() <= attackRange)
        {
            Attack();
            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage * 1.5f); // Danno aumentato durante la carica
                isAttacking = false;
                EndCharge();
            }
        }
    }

    /// <summary>
    /// Termina la carica
    /// </summary>
    private void EndCharge()
    {
        isCharging = false;
        speed = baseSpeed;
    }

    /// <summary>
    /// Effetto di preparazione alla carica
    /// </summary>
    private IEnumerator ChargeWindup()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            // Lampeggio arancione per indicare che sta per caricare
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    /// <summary>
    /// Effetto flash per l'attacco veloce
    /// </summary>
    private IEnumerator FastAttackFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f); // Più veloce del normale
            spriteRenderer.color = originalColor;
        }
    }

    /// <summary>
    /// Comportamento idle più aggressivo
    /// </summary>
    private void HandleIdleBehavior()
    {
        // Movimento più frequente e aggressivo rispetto al zombie normale
        if (Random.Range(0f, 1f) < 0.02f) // 2% di possibilità ogni frame
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 wanderTarget = (Vector2)transform.position + randomDirection * 3f;
            Move(wanderTarget, speed * 0.6f); // Più veloce anche quando vaga
        }
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.ZombieFast;
    }

    /// <summary>
    /// Override per comportamento specifico dopo riposizionamento
    /// </summary>
    protected override void ResetStateAfterRepositioning()
    {
        base.ResetStateAfterRepositioning();
        isCharging = false;
        speed = baseSpeed;
    }

    // Visualizzazione debug
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Disegna il raggio di carica
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);
        
        // Disegna il target della carica se sta caricando
        if (isCharging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, chargeTarget);
            Gizmos.DrawSphere(chargeTarget, 0.5f);
        }
    }
}