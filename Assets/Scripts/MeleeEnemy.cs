using System;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [SerializeField] private float chaseRange = 7f;
    [SerializeField] private float attackRange = 1f;
    
    // Sovrascriviamo il comportamento principale
    protected override void HandleBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();
        
        // Se il giocatore è nel raggio di attacco
        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            Attack();
            
            // Se siamo in fase di attacco, infliggiamo danno
            if (isAttacking && Time.time >= lastAttackTime)
            {
                DamagePlayer(damage);
                isAttacking = false; // Reset dello stato di attacco
            }
        }
        // Se il giocatore è nel raggio di inseguimento
        else if (distanceToPlayer <= chaseRange)
        {
            Move(player.position, speed);
            // LookAtPlayer();
        }
        else
        {
            StopMoving();
        }
    }
    
    // Sovrascriviamo il metodo di attacco per un comportamento specifico
    protected override void Attack()
    {
        base.Attack();
        
        // Implementazione specifica del nemico corpo a corpo
        // Ad esempio, potremmo voler cambiare temporaneamente il colore
        if (isAttacking)
        {
            spriteRenderer.color = Color.red;
            // Potremmo anche aggiungere un'animazione di attacco
        }
        else{
            spriteRenderer.color = Color.blue;
        }
    }
    
    // Override del metodo di OnTriggerEnter2D per gestire logica specifica
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        // Logica specifica aggiuntiva per questo tipo di nemico
    }
}