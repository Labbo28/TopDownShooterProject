using UnityEngine;

[CreateAssetMenu(fileName = "AreaWeaponUpgradeDuration", menuName = "Upgrades/AreaWeaponUpgradeDuration")]
public class AreaWeaponUpgradeDuration : PlayerUpgrade
{
    [Header("Duration Settings")]
    [SerializeField] private float durationMultiplier = 1.1f; // +10% duration per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        AreaWeaponStatsModifier areaStats = player.GetComponent<AreaWeaponStatsModifier>();
        if (areaStats == null)
        {
            areaStats = player.gameObject.AddComponent<AreaWeaponStatsModifier>();
        }
        areaStats.AddDurationMultiplier(durationMultiplier);
        Debug.Log($"Area Weapon Upgrade Duration applied! Level: {currentLevel + 1}");
        Debug.Log($"Current stats - Duration: x{areaStats.DurationMultiplier:F2}");
    }
}
