using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUpgrade", menuName = "Upgrades/SpeedUpgrade")]
public class SpeedUpgrade : PlayerUpgrade
{
    public float speedIncreasePercentage = 1.20f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        float oldSpeed = player.GetMovementSpeed();
        float newSpeed = oldSpeed * speedIncreasePercentage;
        player.SetMovementSpeed(newSpeed);
    }
}