using UnityEngine;
using System.Collections;

/// <summary>
/// Scheletro - Nemico agile con attacchi precisi e movimento tattico
/// </summary>
public class SkeletonEnemy : EnemyBase
{
    [Header("Skeleton Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float dodgeRange = 2f; // Distanza per schivare
    [SerializeField] private float dodgeSpeed = 6f;
    [SerializeField] private float dodgeChance = 0.3f; // 30% possibilità di schivare
    [SerializeField] private float circleDistance = 3f; // Distanza per circondare il player
    
    private bool isDodging = false;
    private bool isCircling = false;
    private float circleAngle = 0f;
    private Vector3 dodgeTarget;
    private float lastDodgeTime = 0f;
    private float dodgeCooldown = 2f;

    protected override void Start()
    {
        base.Start();
        
        // Skeleton settings - equilibrato ma agile
        if (damage <= 5f) damage = 7f;       // Danno buono
        if (attackCooldown <= 1f) attackCooldown = 1.0f;  // Attacco normale
        if (speed <= 3f) speed = 4f; // Più veloce del melee base
        
        // Inizializza angolo casuale per il movimento circolare
        circleAngle = Random.Range(0f, 360f);
    }

    protected override void HandleBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();

        // Se sta schivando, continua il movimento di schivata
        if (isDodging)
        {
            HandleDodge();
            return;
        }

        // Se il player è nel raggio di attacco
        if (distanceToPlayer <= attackRange)
        {
            // Possibilità di schivare prima di attaccare
            if (Random.Range(0f, 1f) < dodgeChance && Time.time >= lastDodgeTime + dodgeCooldown)
            {
                StartDodge();
                return;
            }

            StopMoving();
            Attack();

            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage);
                isAttacking = false;
                
                // Effetto di attacco preciso
                StartCoroutine(PrecisionAttackFlash());
            }
        }
        // Se è nella distanza giusta, circonda il player
        else if (distanceToPlayer <= circleDistance && distanceToPlayer > attackRange)
        {
            HandleCircling();
        }
        // Se è troppo lontano, avvicinati
        else if (distanceToPlayer <= detectRadius)
        {
            Move(player.position, speed);
            isCircling = false;
        }
        else
        {
            HandleIdleBehavior();
            isCircling = false;
        }
    }

    /// <summary>
    /// Inizia una manovra di schivata
    /// </summary>
    private void StartDodge()
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        
        // Calcola direzione di schivata (perpendicolare al player)
        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 dodgeDirection = new Vector2(-toPlayer.y, toPlayer.x); // Perpendicolare
        
        // Scegli casualmente sinistra o destra
        if (Random.Range(0f, 1f) < 0.5f)
            dodgeDirection = -dodgeDirection;
            
        dodgeTarget = transform.position + (Vector3)dodgeDirection * dodgeRange;
        
        // Effetto visivo di schivata
        StartCoroutine(DodgeEffect());
    }

    /// <summary>
    /// Gestisce il movimento durante la schivata
    /// </summary>
    private void HandleDodge()
    {
        float distanceToTarget = Vector3.Distance(transform.position, dodgeTarget);
        
        if (distanceToTarget <= 0.3f)
        {
            isDodging = false;
            return;
        }
        
        Move(dodgeTarget, dodgeSpeed);
    }

    /// <summary>
    /// Movimento circolare attorno al player
    /// </summary>
    private void HandleCircling()
    {
        isCircling = true;
        
        // Incrementa l'angolo per movimento circolare
        circleAngle += Time.deltaTime * 60f; // 60 gradi al secondo
        if (circleAngle >= 360f) circleAngle -= 360f;
        
        // Calcola posizione circolare
        float radians = circleAngle * Mathf.Deg2Rad;
        Vector3 circlePosition = player.position + new Vector3(
            Mathf.Cos(radians) * circleDistance,
            Mathf.Sin(radians) * circleDistance,
            0f
        );
        
        Move(circlePosition, speed * 0.8f);
        
        // Possibilità di cambiare direzione
        if (Random.Range(0f, 1f) < 0.01f) // 1% ogni frame
        {
            circleAngle += Random.Range(90f, 180f); // Cambia direzione
        }
    }

    /// <summary>
    /// Effetto visivo per la schivata
    /// </summary>
    private IEnumerator DodgeEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            Color dodgeColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            
            spriteRenderer.color = dodgeColor; // Diventa semi-trasparente
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = originalColor;
        }
    }

    /// <summary>
    /// Effetto di attacco preciso
    /// </summary>
    private IEnumerator PrecisionAttackFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.cyan; // Colore diverso per indicare precisione
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = originalColor;
        }
    }

    /// <summary>
    /// Comportamento idle tattico
    /// </summary>
    private void HandleIdleBehavior()
    {
        // Movimento tattico - si posiziona strategicamente
        if (Random.Range(0f, 1f) < 0.015f) // 1.5% ogni frame
        {
            Vector2 tacticalDirection = Random.insideUnitCircle.normalized;
            Vector2 tacticalTarget = (Vector2)transform.position + tacticalDirection * 2.5f;
            Move(tacticalTarget, speed * 0.7f);
        }
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Skeleton;
    }

    /// <summary>
    /// Override per comportamento specifico dopo riposizionamento
    /// </summary>
    protected override void ResetStateAfterRepositioning()
    {
        base.ResetStateAfterRepositioning();
        isDodging = false;
        isCircling = false;
        circleAngle = Random.Range(0f, 360f);
    }

    // Visualizzazione debug
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        if (player == null) return;
        
        // Disegna il raggio di circondazione
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, circleDistance);
        
        // Disegna il raggio di schivata
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dodgeRange);
        
        // Disegna il target di schivata se sta schivando
        if (isDodging)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, dodgeTarget);
            Gizmos.DrawSphere(dodgeTarget, 0.3f);
        }
        
        // Disegna la posizione di circondazione se sta circondando
        if (isCircling)
        {
            float radians = circleAngle * Mathf.Deg2Rad;
            Vector3 circlePosition = player.position + new Vector3(
                Mathf.Cos(radians) * circleDistance,
                Mathf.Sin(radians) * circleDistance,
                0f
            );
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(circlePosition, 0.3f);
        }
    }
}