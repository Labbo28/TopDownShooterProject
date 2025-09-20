using UnityEngine;
using System.Collections;

/// <summary>
/// Nemico Zombie - Lento ma resistente, insegue il player con determinazione
/// </summary>
public class ZombieEnemy : EnemyBase
{
    [Header("Zombie Settings")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float pursuingSpeedMultiplier = 1.2f; // Diventa più veloce quando vede il player
    [SerializeField] private bool isPursuing = false;
    
    private float baseSpeed;
    
    protected override void Start()
    {
        base.Start();
        baseSpeed = speed;
    }

    protected override void HandleBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();
        
        // Vampire Survivors-like: Always pursue player
        if (!isPursuing)
        {
            isPursuing = true;
            speed = baseSpeed * pursuingSpeedMultiplier;
        }

        // Se il player è nel raggio di attacco
        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            Attack();

            // Se siamo in fase di attacco, infliggiamo danno
            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage);
                isAttacking = false;
                
                // Effetto visivo di attacco (anche senza animazione specifica)
                StartCoroutine(AttackFlash());
            }
        }
        else
        {
            // Always move toward player
            Move(player.position, speed);
        }
    }
    
    /// <summary>
    /// Comportamento quando il zombie non sta inseguendo il player
    /// </summary>
    private void HandleIdleBehavior()
    {
        // Movimento casuale molto lento
        if (Random.Range(0f, 1f) < 0.01f) // 1% di possibilità ogni frame
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 wanderTarget = (Vector2)transform.position + randomDirection * 2f;
            Move(wanderTarget, speed * 0.3f); // Molto lento quando vaga
        }
    }

    /// <summary>
    /// Effetto flash quando attacca (compensazione per mancanza di animazione attacco)
    /// </summary>
    private IEnumerator AttackFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        
        // Suono di attacco zombie se disponibile
        // AudioManager.Instance?.PlayZombieAttack();
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Zombie;
    }
    
    /// <summary>
    /// Override per comportamento specifico dopo riposizionamento
    /// </summary>
    protected override void ResetStateAfterRepositioning()
    {
        base.ResetStateAfterRepositioning();
        isPursuing = false;
        speed = baseSpeed;
    }
}