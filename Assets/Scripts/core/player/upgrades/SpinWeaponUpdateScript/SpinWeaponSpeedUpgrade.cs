using UnityEngine;

[CreateAssetMenu(fileName = "SpinWeaponSpeedUpgrade", menuName = "Upgrades/SpinWeaponSpeedUpgrade")]
public class SpinWeaponSpeedUpgrade : PlayerUpgrade
{
    [Header("Speed Settings")]
    [SerializeField] private float speedMultiplier = 1.03f; // +3% rotation speed per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        SpinWeaponStatsModifier spinStats = player.GetComponent<SpinWeaponStatsModifier>();
        if (spinStats == null)
        {
            spinStats = player.gameObject.AddComponent<SpinWeaponStatsModifier>();
        }
        spinStats.AddSpeedMultiplier(speedMultiplier);
    }
}
