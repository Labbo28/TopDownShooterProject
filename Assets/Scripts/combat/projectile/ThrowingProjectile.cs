using UnityEngine;
using System.Collections;

public class ThrowingProjectile : MonoBehaviour
{
    [Header("Throwing Projectile Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float maxRange = 4f;
    [SerializeField] private float rotationSpeed = 540f; // Gradi al secondo
    [SerializeField] private int maxPenetrations = 3;
    [SerializeField] private float returnDelay = 0.2f;
    
    private Vector3 startPosition;
    private Vector3 targetDirection;
    private Transform playerTransform;
    private bool isReturning = false;
    private bool hasReachedMaxRange = false;
    private int currentPenetrations = 0;
    private float travelDistance = 0f;
    
    // Set per tracciare nemici già colpiti (evita multi-hit)
    private System.Collections.Generic.HashSet<GameObject> hitEnemies = 
        new System.Collections.Generic.HashSet<GameObject>();

    public void Initialize(Vector3 direction)
    {
        startPosition = transform.position;
        targetDirection = direction.normalized;
        playerTransform = FindObjectOfType<Player>()?.transform;
        
        if (playerTransform == null)
        {
            Debug.LogWarning("Player non trovato! ThrowingProjectile non può funzionare correttamente.");
            Destroy(gameObject, 1f);
            return;
        }
        
        // Timeout di sicurezza
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        CheckReturnConditions();
    }

    private void HandleMovement()
    {
        if (!isReturning)
        {
            // Movimento in avanti
            Vector3 movement = targetDirection * speed * Time.deltaTime;
            transform.position += movement;
            travelDistance += movement.magnitude;
        }
        else
        {
            // Movimento verso il player
            if (playerTransform != null)
            {
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                transform.position += directionToPlayer * speed * 1.2f * Time.deltaTime; // Più veloce al ritorno
                
                // Distruggi quando raggiunge il player
                if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void HandleRotation()
    {
        // Rotazione continua della sprite
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void CheckReturnConditions()
    {
        // Inizia ritorno se ha raggiunto max range o max penetrazioni
        if (!isReturning && !hasReachedMaxRange)
        {
            if (travelDistance >= maxRange || currentPenetrations >= maxPenetrations)
            {
                hasReachedMaxRange = true;
                StartCoroutine(StartReturn());
            }
        }
    }

    private IEnumerator StartReturn()
    {
        yield return new WaitForSeconds(returnDelay);
        isReturning = true;
        
        // Reset hit enemies per permettere danno al ritorno
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Evita multi-hit dello stesso nemico nella stessa direzione
            if (hitEnemies.Contains(other.gameObject))
                return;
                
            hitEnemies.Add(other.gameObject);
            
            // Applica danno
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                float finalDamage = GetFinalDamage();
                damageable.TakeDamage(finalDamage);
                
                // Effetti visivi/sonori
                AudioManager.Instance?.PlayHitSound();
                
                currentPenetrations++;
                
                // Debug info
                Debug.Log($"ThrowingProjectile hit {other.name} for {finalDamage} damage. Penetrations: {currentPenetrations}/{maxPenetrations}");
            }
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            // Rimbalza sui muri o inizia ritorno
            if (!isReturning)
            {
                hasReachedMaxRange = true;
                StartCoroutine(StartReturn());
            }
        }
    }

    private float GetFinalDamage()
    {
        float finalDamage = damage;
        
        // Applica modificatori del player se disponibili
        if (Player.Instance != null)
        {
            // Usa i modificatori ranged per ora (potresti creare modificatori specifici dopo)
            RangedWeaponStatsModifier modifier = Player.Instance.GetComponent<RangedWeaponStatsModifier>();
            if (modifier != null)
            {
                finalDamage *= modifier.DamageMultiplier;
            }
        }
        
        return finalDamage;
    }

    // Metodi per upgrades futuri
    public void SetDamage(float newDamage) => damage = newDamage;
    public void SetSpeed(float newSpeed) => speed = newSpeed;
    public void SetMaxRange(float newRange) => maxRange = newRange;
    public void SetMaxPenetrations(int newPenetrations) => maxPenetrations = newPenetrations;
    public void SetRotationSpeed(float newRotationSpeed) => rotationSpeed = newRotationSpeed;
    
    public float GetDamage() => damage;
    public float GetSpeed() => speed;
    public float GetMaxRange() => maxRange;
    public int GetMaxPenetrations() => maxPenetrations;
}