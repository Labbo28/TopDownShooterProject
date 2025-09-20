using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    private float baseMaxHealth;
    
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;
    
    // IDamageable implementation
    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    
    private void Awake()
    {
    baseMaxHealth = maxHealth;
    ResetHealth();
    InitializeEvents();
    }

    private void InitializeEvents()
    {
        // Initialize events if they're null
        if (onDamaged == null) onDamaged = new UnityEvent();
        if (onHealed == null) onHealed = new UnityEvent();
        if (onDeath == null) onDeath = new UnityEvent();
    }
    
    public void TakeDamage(float damageAmount)
    {
        if (!IsAlive) return; // Non prendere danno se già morto
        
        bool isPlayer = gameObject.CompareTag("Player"); // Check if the damaged entity is the player
        DamageNumberController.Instance.CreateNumber(damageAmount, transform.position + Vector3.up * 0.8f, isPlayer);// Show damage number above the entity
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0
        
        onDamaged?.Invoke();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (!IsAlive) return; // Non curare se morto
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maximum
        
        onHealed?.Invoke();
    }
    
    public virtual void Die()
    {
        if (currentHealth > 0) return; // Assicurati che sia effettivamente morto
        
        onDeath?.Invoke();
        // Base implementation - specific behavior should be handled by subscribers
    }

    public void ScaleHealth(float scaleFactor)
    {
        maxHealth *= scaleFactor;
        // Scala anche la vita corrente proporzionalmente
        float healthPercentage = GetHealthPercentage();
        currentHealth = maxHealth * healthPercentage;
    }

    // Metodo per resettare completamente la salute
    public void ResetHealth()
    {
    maxHealth = baseMaxHealth;
    currentHealth = maxHealth;
    }

    // Metodo per resettare la salute con un nuovo valore massimo
    public void ResetHealth(float newMaxHealth)
    {
    baseMaxHealth = newMaxHealth;
    maxHealth = newMaxHealth;
    currentHealth = maxHealth;
    }

    // Metodo per impostare la salute a un valore specifico
    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth);
        
        if (currentHealth <= 0 && IsAlive)
        {
            Die();
        }
    }

    // Metodo per revivere il personaggio
    public void Revive(float healthAmount = -1)
    {
        if (healthAmount < 0)
        {
            currentHealth = maxHealth; // Revive con salute piena
        }
        else
        {
            currentHealth = Mathf.Min(healthAmount, maxHealth);
        }
        
    }
    
    // Helper methods to get health values
    public float GetHealthPercentage()
    {
        if (maxHealth == 0) return 0;
        return currentHealth / maxHealth;
    }

   public void SetMaxHealth(float newMaxHealth)
   {
       baseMaxHealth = newMaxHealth;
       maxHealth = newMaxHealth;
       currentHealth = maxHealth; // Reset current health to new max
   }
}