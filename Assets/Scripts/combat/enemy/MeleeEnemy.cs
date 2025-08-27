using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MeleeEnemy : EnemyBase
{
    
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
        else
        {
            Move(player.position, speed);
        }

    }
    
    // Sovrascriviamo il metodo di attacco per un comportamento specifico
    protected override void Attack()
    {
        base.Attack();
        
    }

    

    // Override del metodo di OnTriggerEnter2D per gestire logica specifica
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    // Sovrascriviamo il metodo per ottenere il tipo di nemico
    public override EnemyType GetEnemyType()
    {
        return EnemyType.Melee;
    }
}