using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    // Eventi per le animazioni
    public event EventHandler OnEnemyMoving;
    public event EventHandler OnEnemyStopMoving;
    public event EventHandler OnEnemyAttacking;
    
    // Variabili di base per tutti i nemici
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float detectRadius = 10f;
    [SerializeField] protected float actionRadius = 0.5f;

    // Componenti comuni
    protected Transform player;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    
    // Stato del nemico
    protected bool isMoving;
    protected bool isAttacking;  
    protected bool playerInRange = false;
    protected float lastAttackTime;

    // Proprietà dell'interfaccia IDamageable
    public float Health { get => health; protected set => health = value; }
    public float MaxHealth { get => maxHealth; }
    public bool isAlive => Health > 0;

    // Inizializzazione dei componenti
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Inizializzazione dei riferimento al Player
    protected virtual void Start()
    {
        player = FindObjectOfType<Player>()?.transform;
        lastAttackTime = -attackCooldown; // Permette di attaccare subito
    }

    // Logica di aggiornamento principale
    protected virtual void Update()
    {
        if (!isAlive || player == null) return;
        
        HandleBehavior();
    }

    // Comportamento base del nemico - da sovrascrivere nelle classi derivate
    protected abstract void HandleBehavior();

    // Metodo per gestire il movimento
    protected virtual void Move(Vector2 targetPosition, float moveSpeed)
    {
        if (!isAlive) return;

        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (!isMoving)
        {
            isMoving = true;
            OnEnemyMoving?.Invoke(this, EventArgs.Empty);
        }
    }

    // Metodo per fermare il movimento
    protected virtual void StopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
            OnEnemyStopMoving?.Invoke(this, EventArgs.Empty);
        }
    }

    // Metodo per guardare verso il giocatore
    protected virtual void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offset = 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - offset));
        }
    }

    // Metodo per attaccare
    protected virtual void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            OnEnemyAttacking?.Invoke(this, EventArgs.Empty);
            lastAttackTime = Time.time;
        }
    }

    // Metodo per calcolare la distanza dal giocatore
    protected float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }

    // Implementazione dell'interfaccia IDamageable
    public virtual void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        // Feedback visivo (opzionale)
        StartCoroutine(FlashColor(Color.red, 0.1f));
        
        if (Health <= 0)
        {
            Die();
        }
    }

    // Metodo per far lampeggiare il nemico quando subisce danni
    protected System.Collections.IEnumerator FlashColor(Color color, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    // Metodo per la morte del nemico
    public virtual void Die()
    {
        GameManager.Instance?.EnemyKilled();
        Destroy(gameObject);
    }

    // Metodo per infliggere danni al giocatore
    protected virtual void DamagePlayer(float damageAmount)
    {
        if (player != null && player.TryGetComponent<IDamageable>(out var damageablePlayer))
        {
            damageablePlayer.TakeDamage(damageAmount);
        }
    }

    // Metodi per gestire le collisioni
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        //qui andrebbero filtrati i proiettili di altri nemici
        //per fare in modo che i nemici non si massacrino tra loro
        else if (other.CompareTag("Projectile"))
        {
            if (other.TryGetComponent<Projectile>(out var projectile))
            {
                TakeDamage(projectile.Damage);
                Destroy(other.gameObject);
            }
        }
    }

  
protected virtual void RaiseAttackEvent()
{
    isAttacking = true;
    OnEnemyAttacking?.Invoke(this, EventArgs.Empty);
}

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }

        
    }
}