using UnityEngine;
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float actionRadius = 0.5f;
    private SpriteRenderer spriteRenderer;
    

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

    void Update()
    {
        if (isAlive && player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                player.position, speed * Time.deltaTime);
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
            spriteRenderer.color = Color.blue;
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