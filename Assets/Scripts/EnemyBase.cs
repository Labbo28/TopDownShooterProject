using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Events for animations
    public event EventHandler OnEnemyMoving;
    public event EventHandler OnEnemyStopMoving;
    public event EventHandler OnEnemyAttacking;
    
    // Base variables for all enemies
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float detectRadius = 10f;
    [SerializeField] protected float actionRadius = 0.5f;

    // Common components
    protected Transform player;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected HealthSystem healthSystem;
    
    // Enemy state
    protected bool isMoving;
    protected bool isAttacking;  
    protected bool playerInRange = false;
    protected float lastAttackTime;

    // Quick access to health status
    public bool IsAlive => healthSystem != null && healthSystem.IsAlive;

    // Component initialization
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Get or add HealthSystem component
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }
        
        // Setup health events
        healthSystem.onDamaged.AddListener(OnDamaged);
        healthSystem.onDeath.AddListener(Die);
    }

  
    protected virtual void Start()
    {
        player = FindObjectOfType<Player>()?.transform;
        lastAttackTime = -attackCooldown; // Allow attacking immediately
    }

    protected virtual void Update()
    {
        if (!IsAlive || player == null) return;
        
        HandleBehavior();
    }

    // comportamento da implementare nelle classi derivate
    protected abstract void HandleBehavior();

    // Movement handling
    protected virtual void Move(Vector2 targetPosition, float moveSpeed)
    {
        if (!IsAlive) return;

        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (!isMoving)
        {
            isMoving = true;
            spriteRenderer.flipX = direction.x < 0;
            OnEnemyMoving?.Invoke(this, EventArgs.Empty);
        }
    }

   
    protected virtual void StopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
            OnEnemyStopMoving?.Invoke(this, EventArgs.Empty);
        }
    }

   
    protected virtual void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            OnEnemyAttacking?.Invoke(this, EventArgs.Empty);
            lastAttackTime = Time.time;
        }
    }

    // Distance to player calculation
    protected float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }

    // Method called when enemy takes damage
    protected virtual void OnDamaged()
    {
        // Visual feedback
        StartCoroutine(FlashColor(Color.red, 0.1f));
    }

    // Visual damage feedback
    protected System.Collections.IEnumerator FlashColor(Color color, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    // Enemy death method
    public virtual void Die()
    {
        GameManager.Instance?.EnemyKilled();
        Destroy(gameObject);
    }

    // Deal damage to player
    protected virtual void DamagePlayer(float damageAmount)
    {
        if (player != null)
        {
            IDamageable playerHealth = player.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    // Collision handling
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else if (other.CompareTag("Projectile"))
        {
            if (other.TryGetComponent<Projectile>(out var projectile) && healthSystem != null)
            {
                healthSystem.TakeDamage(projectile.Damage);
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