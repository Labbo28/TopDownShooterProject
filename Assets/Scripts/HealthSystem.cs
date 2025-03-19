using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100; // Numero da impostare
     private float currentHealth {get;set;}
    
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // Non deve scendere sotto
                                                     // lo 0
        
        onDamaged?.Invoke();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Non deve andare oltre al numero massimo
        
        onHealed?.Invoke();
    }
    
    private void Die()
    {
        onDeath?.Invoke();
        //Qua è dove si può settare cosa succede alla morte
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }
    public float getMaxHealth() 
    {
        return maxHealth;
    }
}
