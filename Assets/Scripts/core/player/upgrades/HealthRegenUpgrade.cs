using UnityEngine;

[CreateAssetMenu(fileName = "HealthRegenUpgrade", menuName = "Upgrades/HealthRegenUpgrade")]
public class HealthRegenUpgrade : PlayerUpgrade
{
    [SerializeField] private float regenAmountPerSecond = 2f;
    [SerializeField] private float regenInterval = 1f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Aggiungi o aggiorna il componente di rigenerazione
        HealthRegenComponent regenComponent = player.GetComponent<HealthRegenComponent>();
        
        if (regenComponent == null)
        {
            regenComponent = player.gameObject.AddComponent<HealthRegenComponent>();
        }
        
        // Aumenta la quantità di rigenerazione per livello
        float totalRegenPerSecond = regenAmountPerSecond * (currentLevel + 1);
        regenComponent.SetRegeneration(totalRegenPerSecond, regenInterval);
        
    }
}