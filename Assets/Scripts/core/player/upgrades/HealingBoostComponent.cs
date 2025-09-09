using UnityEngine;

public class HealingBoostComponent : MonoBehaviour
{
    private float healingMultiplier = 1f;
    private HealthSystem healthSystem;

    public float HealingMultiplier => healingMultiplier;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealingBoostComponent requires a HealthSystem component!");
            Destroy(this);
            return;
        }

        // Sostituisci il metodo Heal originale con uno potenziato
        // Questo richiederà una modifica al HealthSystem o un override del comportamento
    }

    public void AddHealingMultiplier(float multiplier)
    {
        healingMultiplier *= multiplier;
        Debug.Log($"Total healing multiplier: {healingMultiplier}");
    }

    // Metodo per applicare la cura potenziata
    public void EnhancedHeal(float amount)
    {
        if (healthSystem != null)
        {
            float enhancedAmount = amount * healingMultiplier;
            healthSystem.Heal(enhancedAmount);
            
            Debug.Log($"Enhanced healing applied: {amount} -> {enhancedAmount} (multiplier: {healingMultiplier})");
        }
    }

    // Resetta il moltiplicatore (utile per il reset del gioco)
    public void ResetMultiplier()
    {
        healingMultiplier = 1f;
    }
}