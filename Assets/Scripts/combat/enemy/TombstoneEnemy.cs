using UnityEngine;

public class TombstoneEnemy : EnemyBase
{
    [SerializeField] private float minDistance = 3f;  // Distanza minima dal giocatore
    [SerializeField] private float maxDistance = 7f;  // Distanza massima dal giocatore
    [SerializeField] private float shootingRange = 10f;  // Raggio di tiro
    [SerializeField] private GameObject projectilePrefab;  // Prefab del proiettile

    public override EnemyType GetEnemyType()
    {
       return EnemyType.Tombstone;
    }

    protected override void HandleBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();
        
        // Gestione del movimento basato sulla distanza
        if (distanceToPlayer > maxDistance)
        {
            // Avviciniamoci al giocatore
            Move(player.position, speed);
        }
        else if (distanceToPlayer < minDistance)
        {
            // Allontaniamoci dal giocatore
            Vector2 directionFromPlayer = (transform.position - player.position).normalized;
            Vector2 retreatPosition = (Vector2)transform.position + directionFromPlayer * speed;
            Move(retreatPosition, speed);
        }
        else
        {
            // Siamo alla distanza ideale, fermiamoci
            StopMoving();
        }
        
        // Orientamento verso il giocatore
        //LookAtPlayer();
        
        // Tentativo di sparare se siamo in raggio
        if (distanceToPlayer <= shootingRange && Time.time >= lastAttackTime + attackCooldown)
        {
            ShootAtPlayer();
        }
    }
    
    protected virtual void ShootAtPlayer()
    {
        // Resettiamo il timer di attacco
        lastAttackTime = Time.time;
        
        // Emettiamo l'evento di attacco
       RaiseAttackEvent();
        
        // Calcoliamo la direzione verso il giocatore
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Creiamo e configuriamo il proiettile
        GameObject projectile = Instantiate(
            projectilePrefab, 
            transform.position, 
            Quaternion.Euler(0, 0, angle - 90f)
        );

        
        // Ricordiamo di cancellare lo stato di attacco dopo un breve periodo
        Invoke(nameof(ResetAttackState), 0.2f);
    }
    
    private void ResetAttackState()
    {
        isAttacking = false;
    }
}