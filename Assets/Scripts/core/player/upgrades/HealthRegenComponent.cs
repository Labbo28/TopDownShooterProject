using UnityEngine;
using System.Collections;

public class HealthRegenComponent : MonoBehaviour
{
    private float regenAmountPerSecond = 0f;
    private float regenInterval = 1f;
    private HealthSystem healthSystem;
    private Coroutine regenCoroutine;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Destroy(this);
        }
    }

    public void SetRegeneration(float amountPerSecond, float interval)
    {
        regenAmountPerSecond = amountPerSecond;
        regenInterval = interval;
        
        // Riavvia la rigenerazione con i nuovi valori
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        
        if (regenAmountPerSecond > 0f)
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);
            
            if (healthSystem != null && healthSystem.IsAlive && healthSystem.Health < healthSystem.MaxHealth)
            {
                healthSystem.Heal(regenAmountPerSecond * regenInterval);
            }
        }
    }

    private void OnDestroy()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
    }
}