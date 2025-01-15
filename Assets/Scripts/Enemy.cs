using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    //eventi per gestire le diverse animazioni del nemico
    public event EventHandler OnEnemyMoving;
    public event EventHandler OnEnemyStopMoving;
    public event EventHandler OnEnemyAttacking;
    
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float actionRadius = 0.5f;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private bool isAttacking;
    

    public float Health { get; private set; } = 100f;
    public float MaxHealth { get; private set; } = 100f;
    public bool isAlive => Health > 0;

    private bool playerInRange = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player = FindObjectOfType<Player>().transform;
      
    }

    private void HandleEnemyMovement()
    {
        if (isAlive && player != null)
        {
            LookAtPlayer();
            isMoving = true;
            transform.position = Vector2.MoveTowards(transform.position, 
                player.position, speed * Time.deltaTime);
            OnEnemyMoving?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            isMoving = false;
            OnEnemyStopMoving?.Invoke(this, EventArgs.Empty);
        }
    }

    void Update()
    {
       HandleEnemyMovement();
    }

   
    private void LookAtPlayer()
    {
        if (player != null)
        {
            // Calcola la direzione verso il giocatore
            Vector3 direction = player.position - transform.position;

            // Calcola l'angolo in radianti e converti in gradi
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offset = 90f;
            // Imposta la rotazione del nemico lungo l'asse Z
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - offset));
        }
    }



    



    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        { 
            TakeDamage(other.GetComponent<Projectile>().Damage);
            Destroy(other.gameObject);
            
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player in range");
            isAttacking = true;
            OnEnemyAttacking?.Invoke(this, EventArgs.Empty);
            playerInRange = true;
            InvokeRepeating(nameof(InflictDamageToPlayer), 0f, 1f);
            spriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player out of range");
            playerInRange = false;
            CancelInvoke(nameof(InflictDamageToPlayer));
        }
    }

    public void InflictDamageToPlayer()
    {
        if (playerInRange && player != null && player.TryGetComponent<IDamageable>(out var damageablePlayer))
        {
            damageablePlayer.TakeDamage((int)damage);
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}