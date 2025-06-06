using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Runtime.CompilerServices;

public enum EnemyType
{
    Melee,
    Ranged,
    Sniper,
    Boss,
    // altri tipi di nemici
}

public class EnemyDeadEvent : UnityEvent<EnemyType, Vector3> { }

public abstract class EnemyBase : MonoBehaviour
{
    // Events for animations
    public UnityEvent OnEnemyhit;
    public EnemyDeadEvent OnEnemyDead;

    public class EnemyDeadEventArgs : EventArgs
    {
        public EnemyType EnemyType { get; }

        private Vector3 position;
        public Vector3 Position => position;

        public EnemyDeadEventArgs(EnemyType enemyType, Vector3 position)
        {
            EnemyType = enemyType;
            this.position = position;
        }
    }

    // Base variables for all enemies
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float detectRadius = 10f;
    [SerializeField] protected float actionRadius = 0.5f;
    [SerializeField] protected Image HealthBar;

    [SerializeField] protected Image backgroundHealthBar;
    
    // Durata in secondi in cui la healthbar rimane visibile dopo aver subito danni
    [SerializeField] protected float healthBarDisplayDuration = 2f;

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
    
    // Coroutine reference per gestire il timer della healthbar
    private Coroutine hideHealthBarCoroutine;

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

        // Initialize events
        OnEnemyhit = new UnityEvent();
        OnEnemyDead = new EnemyDeadEvent();

        // Initialize health bar
        HealthBar.fillAmount = healthSystem.GetHealthPercentage();
        // Disabilita la healthbar di default
        HealthBar.gameObject.SetActive(false);
        backgroundHealthBar.gameObject.SetActive(false);
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

    protected abstract EnemyType GetEnemyType();

    // comportamento da implementare nelle classi derivate
    protected abstract void HandleBehavior();

    // Movement handling
    protected virtual void Move(Vector2 targetPosition, float moveSpeed)
    {
        if (!IsAlive) return;

        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        spriteRenderer.flipX = direction.x < 0;
        
        if (!isMoving)
        {
            isMoving = true;
        }
    }
   
    protected virtual void StopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
        }
    }
   
    protected virtual void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
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
        // Visual feedback: flash del colore
        OnEnemyhit?.Invoke();
        // Aggiorna la healthbar
        HealthBar.fillAmount = healthSystem.GetHealthPercentage();
        // Rendi visibile la healthbar
        HealthBar.gameObject.SetActive(true);
        backgroundHealthBar.gameObject.SetActive(true);

        // Se esiste già una coroutine per nascondere la healthbar, fermala
        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }
        // Avvia la coroutine per nascondere la healthbar dopo alcuni secondi
        hideHealthBarCoroutine = StartCoroutine(ShowHealthBarCoroutine());
    }

    // Coroutine per nascondere la healthbar dopo un certo tempo
    protected IEnumerator ShowHealthBarCoroutine()
    {
        yield return new WaitForSeconds(healthBarDisplayDuration);
        HealthBar.gameObject.SetActive(false);
        backgroundHealthBar.gameObject.SetActive(false);
    }

    private void DisableHealthBar()
    {
        HealthBar.gameObject.SetActive(false);
        backgroundHealthBar.gameObject.SetActive(false);
    }

    // Enemy death method
    public virtual void Die()
    {
        GameManager.Instance?.EnemyKilled();
        OnEnemyDead?.Invoke(GetEnemyType(), transform.position);
        StartCoroutine(HandleDeath());
        DisableHealthBar();
    }

    private IEnumerator HandleDeath()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        spriteRenderer.enabled = false; 
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
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
