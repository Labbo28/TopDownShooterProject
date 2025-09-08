using UnityEngine;

[CreateAssetMenu(fileName = "MaxHealthUpgrade", menuName = "Upgrades/MaxHealthUpgrade")]
public class MaxHealthUpgrade : PlayerUpgrade
{
    public float healthIncreasePercentage = 1.2f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        HealthSystem hs = player.GetComponent<HealthSystem>();
        hs.ScaleHealth(healthIncreasePercentage);
        hs.onHealed?.Invoke();
    }
}