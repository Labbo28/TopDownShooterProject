using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponReloadSpeedUpgrade", menuName = "Upgrades/RangedWeaponReloadSpeedUpgrade")]
public class RangedWeaponReloadSpeedUpgrade : PlayerUpgrade
{
    [Header("Reload Settings")]
    [SerializeField] private float reloadSpeedMultiplier = 0.9f; // Riduce tempo di ricarica

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        RangedWeaponStatsModifier rangedStats = player.GetComponent<RangedWeaponStatsModifier>();
        if (rangedStats == null)
        {
            rangedStats = player.gameObject.AddComponent<RangedWeaponStatsModifier>();
        }
        rangedStats.AddReloadSpeedMultiplier(reloadSpeedMultiplier);
    }
}
