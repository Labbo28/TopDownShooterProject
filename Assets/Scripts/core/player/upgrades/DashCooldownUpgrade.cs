
using UnityEngine;

[CreateAssetMenu(fileName = "DashCooldownUpgrade", menuName = "Upgrades/DashCooldownUpgrade")]
public class DashCooldownUpgrade : PlayerUpgrade
{
    public float dashCooldownReducePercentage = 0.8f;

    public override void ApplyUpgrade(Player player)
    {   // Controlla se l'upgrade è già al livello massimo
        if (IsMaxLevel) return;

        float oldDashCooldown=player.GetDashCooldown();
        float newDashCooldown= oldDashCooldown * dashCooldownReducePercentage;
        player.GetDashTimer().Reset(newDashCooldown);

        // Incrementa il livello dell'upgrade
        currentLevel++;
    }


}