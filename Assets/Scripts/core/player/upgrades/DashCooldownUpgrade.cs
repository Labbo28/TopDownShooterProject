using UnityEngine;

[CreateAssetMenu(fileName = "DashCooldownUpgrade", menuName = "Upgrades/DashCooldownUpgrade")]
public class DashCooldownUpgrade : PlayerUpgrade
{
    public float dashCooldownReducePercentage = 0.8f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        float oldDashCooldown = player.GetDashCooldown();
        float newDashCooldown = oldDashCooldown * dashCooldownReducePercentage;
        player.SetDashCooldown(newDashCooldown);
    }
}