
using UnityEngine;

[CreateAssetMenu(fileName = "MaxHealthUpgrade", menuName = "Upgrades/MaxHealthUpgrade")]
public class MaxHealthUpgrade : PlayerUpgrade
{
    public float healthIncreasePercentage = 1.2f;

    public override void ApplyUpgrade(Player player)
    {   // Controlla se l'upgrade è già al livello massimo
        if (IsMaxLevel) return;

        HealthSystem hs = player.GetComponent<HealthSystem>();
        hs.ScaleHealth(healthIncreasePercentage);
        hs.onHealed?.Invoke();
        


        // Incrementa il livello dell'upgrade
        currentLevel++;
    }


}