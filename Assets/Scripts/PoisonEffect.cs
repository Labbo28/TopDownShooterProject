using System.Collections;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private float duration;
    private float tickRate;
    private float damagePerTick;
    private Color poisonColor;
    
    private IDamageable damageableTarget;
    private SpriteRenderer targetSpriteRenderer;
    private Color originalColor;
    private Coroutine poisonCoroutine;
    
    private void Awake()
    {
        // Get the IDamageable component
        damageableTarget = GetComponent<IDamageable>();
        
        // Get the SpriteRenderer for visual effect
        targetSpriteRenderer = GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer != null)
        {
            originalColor = targetSpriteRenderer.color;
        }
    }
    
    public void SetPoisonParameters(float newDuration, float newTickRate, float newDamagePerTick, Color newPoisonColor)
    {
        duration = newDuration;
        tickRate = newTickRate;
        damagePerTick = newDamagePerTick;
        poisonColor = newPoisonColor;
        
        // Si ferma se non può applicare veleno
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        
        // Applica nuovo veleno
        poisonCoroutine = StartCoroutine(ApplyPoisonDamage());
    }
    
    private IEnumerator ApplyPoisonDamage()
    {
        float elapsedTime = 0f;
        
        // Colore veleno
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = Color.Lerp(originalColor, poisonColor, 0.3f);
        }
        
        // D.O.T
        while (elapsedTime < duration)
        {
            // Applica danno se effettivamente nemico può ricevere danno
            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(damagePerTick);
                
                // Pulsazione (carina visivamente)
                if (targetSpriteRenderer != null)
                {
                    StartCoroutine(PulseColor());
                }
            }
            
            // Aspetta il nuovo tick
            yield return new WaitForSeconds(tickRate);
            elapsedTime += tickRate;
        }
        
        // Resetta colore
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = originalColor;
        }
        
        
        Destroy(this);
    }
    
    private IEnumerator PulseColor()
    {
        //Pulsazione
        Color pulseColor = Color.Lerp(targetSpriteRenderer.color, poisonColor, 0.7f);
        targetSpriteRenderer.color = pulseColor;
        
        yield return new WaitForSeconds(0.2f);
        
        // Colore veleno originale
        targetSpriteRenderer.color = Color.Lerp(originalColor, poisonColor, 0.3f);
    }
}