using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // Numero da impostare
    [SerializeField] private int currentHealth;
    
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damageAmount)
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
}
