using UnityEngine;

[CreateAssetMenu(fileName = "HealingEfficiencyUpgrade", menuName = "Upgrades/HealingEfficiencyUpgrade")]
public class HealingEfficiencyUpgrade : PlayerUpgrade
{
    [SerializeField] private float healingMultiplier = 1.3f; // Aumenta l'efficacia della cura del 30%

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Aumenta l'efficacia della cura dei Medikit attraverso GameManager
        if (GameManager.Instance != null)
        {
            float currentHealAmount = GameManager.Instance.GetHealAmount();
            float newHealAmount = currentHealAmount * healingMultiplier;
            
            GameManager.Instance.SetHealAmount(newHealAmount);
            
        }
        
        // Aggiungi anche un componente che migliora tutte le fonti di cura
        HealingBoostComponent healingBoost = player.GetComponent<HealingBoostComponent>();
        if (healingBoost == null)
        {
            healingBoost = player.gameObject.AddComponent<HealingBoostComponent>();
        }
        
        healingBoost.AddHealingMultiplier(healingMultiplier);
        
    }
}