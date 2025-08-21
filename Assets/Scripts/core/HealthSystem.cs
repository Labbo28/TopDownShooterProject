using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;
    
    // IDamageable implementation
    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        
        // Initialize events if they're null
        if (onDamaged == null) onDamaged = new UnityEvent();
        if (onHealed == null) onHealed = new UnityEvent();
        if (onDeath == null) onDeath = new UnityEvent();
    }
    
    public void TakeDamage(float damageAmount)
    {
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
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maximum
        
        onHealed?.Invoke();
    }
    
    public virtual void Die()
    {
        onDeath?.Invoke();
        // Base implementation - specific behavior should be handled by subscribers
    }

    public void ScaleHealth(float scaleFactor)
    {
        maxHealth *= scaleFactor;
      
    }
    
    // Helper methods to get health values
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}