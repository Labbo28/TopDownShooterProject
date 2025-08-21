using System.Collections;
using UnityEngine;

public class SniperEnemy : EnemyBase
{
    [Header("Sniper Settings")]
    [SerializeField] private float idealDistance = 15f;      // Distanza dal player ideale
    [SerializeField] private float minDistance = 12f;        // Distanza minima dal player
    [SerializeField] private float aimTime = 3f;             // Tempo per mirare
    [SerializeField] private GameObject bulletPrefab;        // Proiettile
    [SerializeField] private Transform firePoint;            // Point where bullets spawn
    [SerializeField] private LineRenderer aimLaser;          // Effetto visivo laser
    [SerializeField] private Color aimingColor = Color.red;  // Colore rosso per laser
    [SerializeField] private float repositionRange = 5f;     // Distanza di riposizionamento
    
    // Stati possibili dello sniper
    private enum SniperState { Repositioning, Aiming, Shooting }
    private SniperState currentState = SniperState.Repositioning;
    
    private float aimProgress = 0f;
    private Vector2 targetPosition;
    private Vector2 playerLastPosition;
    
    protected override void Start()
    {
        base.Start();
        
        // Default sniper settings if not set in inspector
        if (damage <= 5f) damage = 20f;       // Danno più alto degli altri enemies
        if (attackCooldown <= 1f) attackCooldown = 4f;  // Cooldown più lungo
        if (detectRadius <= 10f) detectRadius = 20f;    // Player detect
        
        // Inizializza laser
        if (aimLaser != null)
        {
            aimLaser.enabled = false;
        }
        
        // Setta il firepoint, lo crea se non c'è
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.right * 0.5f; // Aggiusta posizione
        }
    }
    
    protected override void HandleBehavior()
    {
        if (player == null) return;
        
        playerLastPosition = player.position;
        
        // Guarda sempre player
        FacePlayer();
        
        // State machine
        switch (currentState)
        {
            case SniperState.Repositioning:
                HandleRepositioning();
                break;
                
            case SniperState.Aiming:
                HandleAiming();
                break;
                
            case SniperState.Shooting:
                HandleShooting();
                break;
        }
    }
    
    private void HandleRepositioning()
    {
        // Troppo vicino, riposiziona
        if (DistanceToPlayer() < minDistance)
        {
            // Si sposta
            Vector2 directionFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;
            targetPosition = (Vector2)player.position + directionFromPlayer * idealDistance;
            Move(targetPosition, speed);
        }
        // Distanza è ok? Allora mira
        else
        {
            StopMoving();
            currentState = SniperState.Aiming;
            aimProgress = 0f;
            if (aimLaser != null)
            {
                aimLaser.enabled = true;
            }
        }
    }
    
    private void HandleAiming()
    {
        // Si ferma quando mira
        StopMoving();
        
        // Aggiorna aim progress
        aimProgress += Time.deltaTime / aimTime;
        
        // Aggiorna laser
        UpdateAimLaser();
        
        // Mira completata? Spara Peppino! SPARAAAAA
        if (aimProgress >= 1.0f)
        {
            currentState = SniperState.Shooting;
            RaiseAttackEvent();
        }
    }
    
    private void UpdateAimLaser()
    {
        if (aimLaser != null)
        {
            // Posiziona il laser
            aimLaser.SetPosition(0, firePoint.position);
            aimLaser.SetPosition(1, player.position);
            
            // Colore diventa rosso
            Color currentColor = aimingColor;
            currentColor.a = aimProgress;
            aimLaser.startColor = currentColor;
            aimLaser.endColor = currentColor;
        }
    }
    
    private void HandleShooting()
    {
        // Spara
        FireProjectile();
        
        // Spegni mira laser
        if (aimLaser != null)
        {
            aimLaser.enabled = false;
        }
        
        // Sparato? Riposizionati
        StartCoroutine(RepositionAfterDelay(0.5f));
        
        // Reset state
        currentState = SniperState.Repositioning;
    }
    
    private IEnumerator RepositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Trova nuova posizione random
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        targetPosition = (Vector2)transform.position + randomDirection * repositionRange;
        
        // Assicurati nuova posizione rispetti i limiti imposti
        Vector2 directionToPlayer = ((Vector2)player.position - targetPosition).normalized;
        float distanceToPlayer = Vector2.Distance(targetPosition, player.position);
        
        if (distanceToPlayer < minDistance)
        {
            targetPosition = (Vector2)player.position - directionToPlayer * idealDistance;
        }
    }
    
    private void FireProjectile()
    {
        if (bulletPrefab != null && player != null)
        {
            // Crea proiettile
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // Prende la direzione del player
            Vector2 directionToPlayer = (playerLastPosition - (Vector2)firePoint.position).normalized;
            
            // Ruota il proiettile verso player
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            
            if (bullet.TryGetComponent<Projectile>(out var projectile) && 
                projectile.projectileSO != null)
            {
                // You could modify the projectile SO attributes here if needed
                // For example: projectile.projectileSO.damage = damage;
            }
        }
    }
    
    private void FacePlayer()
    {
        // Flippa la sprite
        if (player != null)
        {
            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, idealDistance);
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Sniper;
    }
}