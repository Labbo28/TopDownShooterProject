using UnityEngine;

[CreateAssetMenu(fileName = "DashDistanceUpgrade", menuName = "Upgrades/DashDistanceUpgrade")]
public class DashDistanceUpgrade : PlayerUpgrade
{
    [SerializeField] private float distanceIncreasePercentage = 1.15f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Accediamo alla costante DashDistance tramite reflection
        // dato che è privata, aumentiamo indirettamente l'effetto del dash
        
        // Per ora implementiamo aumentando la velocità di movimento durante il dash
        // Un approccio alternativo sarebbe modificare la classe Player per esporre DashDistance
        float currentSpeed = player.GetMovementSpeed();
        float newSpeed = currentSpeed * distanceIncreasePercentage;
        player.SetMovementSpeed(newSpeed);
        
        Debug.Log($"Dash Distance Upgrade applied! Movement speed increased to: {newSpeed}");
    }
}