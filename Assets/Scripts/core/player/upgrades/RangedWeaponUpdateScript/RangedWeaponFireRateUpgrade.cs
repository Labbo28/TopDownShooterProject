using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponFireRateUpgrade", menuName = "Upgrades/RangedWeaponFireRateUpgrade")]
public class RangedWeaponFireRateUpgrade : PlayerUpgrade
{
    [Header("Fire Rate Settings")]
    [SerializeField] private float fireRateMultiplier = 0.85f; // Riduce il tempo tra colpi

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        RangedWeaponStatsModifier rangedStats = player.GetComponent<RangedWeaponStatsModifier>();
        if (rangedStats == null)
        {
            rangedStats = player.gameObject.AddComponent<RangedWeaponStatsModifier>();
        }
        rangedStats.AddFireRateMultiplier(fireRateMultiplier);
    }
}
