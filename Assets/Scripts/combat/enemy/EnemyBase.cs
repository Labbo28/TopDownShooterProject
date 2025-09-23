using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

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
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float actionRadius = 0.5f;
    [SerializeField] protected Image HealthBar;
    [SerializeField] protected Image backgroundHealthBar;
    
    // Durata in secondi in cui la healthbar rimane visibile dopo aver subito danni
    [SerializeField] protected float healthBarDisplayDuration = 2f;

    // ===== SISTEMA DI RIPOSIZIONAMENTO =====
    [Header("Repositioning System")]
    [SerializeField] private bool enableRepositioning = true;
    [SerializeField] private float maxDistanceFromPlayer = 25f;
    [SerializeField] private float repositionDistance = 15f;
    [SerializeField] private bool debugRepositioning = false;

    [Header("Dialogue on enemy Death")]
    [Tooltip("⚠️ Se assegni un Dialogue qui, il dialogo verrà mostrato alla morte di TUTTI i nemici di questo tipo (tutti i prefab/istanze che usano questo script). Usa questo campo SOLO per nemici unici che appaiono una sola volta (es: boss o mini-boss unici).")]
    [SerializeField] private Dialogue enemyDeathDialogue; // Assegna da Inspector
    
    
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
         healthSystem.SetMaxHealth(health);

        // Setup health events
        healthSystem.onDamaged.AddListener(OnDamaged);
        healthSystem.onDeath.AddListener(Die);

        // Initialize events
        OnEnemyhit = new UnityEvent();
        OnEnemyDead = new EnemyDeadEvent();

        // Initialize health bar
        if (HealthBar != null)
        {
            HealthBar.fillAmount = healthSystem.GetHealthPercentage();
            HealthBar.gameObject.SetActive(false);
        }
        if (backgroundHealthBar != null)
        {
            backgroundHealthBar.gameObject.SetActive(false);
        }

    }

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>()?.transform;
        lastAttackTime = -attackCooldown; // Allow attacking immediately
    }

    protected virtual void Update()
    {
        if (!IsAlive || GameManager.Instance.CurrentGameState==GameState.GameOver
        || GameManager.Instance.CurrentGameState==GameState.Paused) return;
        
        // Sistema di riposizionamento
        if (enableRepositioning)
        {
            CheckAndRepositionIfNeeded();
        }
        
        HandleBehavior();
    }

    // ===== SISTEMA DI RIPOSIZIONAMENTO =====
    
    /// <summary>
    /// Controlla se il nemico è troppo lontano dal player e lo riposiziona se necessario
    /// </summary>
    private void CheckAndRepositionIfNeeded()
    {
        if (player == null) return;
        
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceFromPlayer > maxDistanceFromPlayer)
        {
            RepositionNearPlayer();
        }
    }
    
    /// <summary>
    /// Riposiziona il nemico in una posizione casuale attorno al player
    /// </summary>
    private void RepositionNearPlayer()
    {
        if (player == null) return;
        
        // Genera un angolo casuale attorno al player
        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        // Calcola la nuova posizione usando trigonometria
        Vector3 repositionOffset = new Vector3(
            Mathf.Cos(randomAngle) * repositionDistance,
            Mathf.Sin(randomAngle) * repositionDistance,
            0f
        );
        
        Vector3 newPosition = player.position + repositionOffset;
        
        // Store old position for debug
        Vector3 oldPosition = transform.position;
        
        // Applica la nuova posizione
        transform.position = newPosition;
        
        // Reset physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Reset enemy state dopo riposizionamento
        ResetStateAfterRepositioning();
        
        if (debugRepositioning)
        {
        }
    }
    
    /// <summary>
    /// Resetta gli stati del nemico dopo il riposizionamento
    /// </summary>
    protected virtual void ResetStateAfterRepositioning()
    {
        isMoving = false;
        isAttacking = false;
        playerInRange = false;
        
        // Le classi derivate possono override questo metodo per comportamenti specifici
    }
    
    /// <summary>
    /// Forza il riposizionamento (utile per testing)
    /// </summary>
    public void ForceReposition()
    {
        if (enableRepositioning)
        {
            RepositionNearPlayer();
        }
    }
    
    /// <summary>
    /// Abilita/disabilita il sistema di riposizionamento
    /// </summary>
    public void SetRepositioningEnabled(bool enabled)
    {
        enableRepositioning = enabled;
    }

    // ===== METODI ORIGINALI =====

    public abstract EnemyType GetEnemyType();

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
        if (HealthBar != null)
        {
            HealthBar.fillAmount = healthSystem.GetHealthPercentage();
            HealthBar.gameObject.SetActive(true);
        }
        if (backgroundHealthBar != null)
        {
            backgroundHealthBar.gameObject.SetActive(true);
        }

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
        if (HealthBar != null) HealthBar.gameObject.SetActive(false);
        if (backgroundHealthBar != null) backgroundHealthBar.gameObject.SetActive(false);
    }

    private void DisableHealthBar()
    {
        if (HealthBar != null) HealthBar.gameObject.SetActive(false);
        if (backgroundHealthBar != null) backgroundHealthBar.gameObject.SetActive(false);
    }

    // Enemy death method
    public virtual void Die()
    {
        GameManager.Instance?.EnemyKilled();
        
        OnEnemyDead?.Invoke(GetEnemyType(), transform.position);
        StartCoroutine(HandleDeath());
        DisableHealthBar();
        // se il enemy è skeleton boss alla sua morte cambia scena


    }

    private IEnumerator HandleDeath()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        spriteRenderer.enabled = false;
        Destroy(gameObject);
        // Avvia il dialogo solo se assegnato
        if (enemyDeathDialogue != null && DialogueManager.Instance != null)
        {   
            
            DialogueManager.Instance.StartDialogue(enemyDeathDialogue);
        }
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
                AudioManager.Instance?.PlayHitSound();
                healthSystem.TakeDamage(projectile.Damage);
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("EnemyProjectile"))
        {
            //ignore collision
            Physics2D.IgnoreCollision(other, GetComponent<Collider2D>());
            
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

    public void ScaleDifficulty(float healthMultiplier, float damageMultiplier, float speedMultiplier)
    {
        if (healthSystem != null)
        {
            healthSystem.ScaleHealthEnemy(healthMultiplier);
            if (HealthBar != null)
            {
                HealthBar.fillAmount = healthSystem.GetHealthPercentage();
            }
        }
        
        damage *= damageMultiplier;
        speed *= speedMultiplier;
        
        if (rb != null)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    // Debug visualization
    protected virtual void OnDrawGizmosSelected()
    {
        if (!enableRepositioning || player == null) return;
        
        // Disegna la distanza massima (rosso)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxDistanceFromPlayer);
        
        // Disegna la distanza di riposizionamento (giallo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, repositionDistance);
        
        // Disegna la linea di connessione
        float currentDistance = Vector3.Distance(transform.position, player.position);
        Gizmos.color = currentDistance > maxDistanceFromPlayer ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, player.position);
        
        // Cerchio attorno al nemico per indicare se è fuori range
        if (currentDistance > maxDistanceFromPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}