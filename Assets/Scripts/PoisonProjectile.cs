using System.Collections;
using UnityEngine;

public class PoisonProjectile : Projectile
{
    [SerializeField] private float poisonDuration = 5f;
    [SerializeField] private float poisonTickRate = 1f;  // Quanto spesso hitta il poison?
    [SerializeField] private float poisonDamagePerTick = 5f;
    [SerializeField] private Color poisonColor = new Color(0.5f, 1f, 0f, 1f);  //Colore verde per segnare lo status di avvelenato
    [SerializeField] private GameObject poisonEffectPrefab;  // VFX opzionale, vedremo se lasciarlo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Applica danno iniziale
            if (other.TryGetComponent<IDamageable>(out var damageableTarget))
            {
                damageableTarget.TakeDamage(Damage);
                
                // Applica veleno
                if (other.TryGetComponent<Player>(out var player))
                {
                    // Se il player è avvelenato, riduci la durata
                    // Altrimenti, aggiungi di nuovo veleno
                    PoisonEffect poisonEffect = player.GetComponent<PoisonEffect>();
                    if (poisonEffect == null)
                    {
                        poisonEffect = player.gameObject.AddComponent<PoisonEffect>();
                    }
                    
                    poisonEffect.SetPoisonParameters(poisonDuration, poisonTickRate, poisonDamagePerTick, poisonColor);
                    
                    // Se player non è avvelenato, setta effetto visivo
                    if (poisonEffectPrefab != null)
                    {
                        GameObject effect = Instantiate(poisonEffectPrefab, player.transform);
                        Destroy(effect, poisonDuration);
                    }
                }
            }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}