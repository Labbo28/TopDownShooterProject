
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUpgrade", menuName = "Upgrades/SpeedUpgrade")]
public class SpeedUpgrade : PlayerUpgrade
{
    public float speedIncreasePercentage = 1.20f;

    public override void ApplyUpgrade(Player player)
    {   // Controlla se l'upgrade è già al livello massimo
        if (IsMaxLevel) return;

        float oldSpeed=player.GetMovementSpeed();
        float newSpeed = oldSpeed * speedIncreasePercentage;
        player.SetMovementSpeed(newSpeed);

        // Incrementa il livello dell'upgrade
        currentLevel++;
    }


}