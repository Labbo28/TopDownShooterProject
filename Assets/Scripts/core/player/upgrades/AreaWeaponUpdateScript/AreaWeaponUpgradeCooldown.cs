using UnityEngine;

[CreateAssetMenu(fileName = "AreaWeaponUpgradeCooldown", menuName = "Upgrades/AreaWeaponUpgradeCooldown")]
public class AreaWeaponUpgradeCooldown : PlayerUpgrade
{
    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownReduction = 0.5f; // -0.5 seconds cooldown per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        AreaWeaponStatsModifier areaStats = player.GetComponent<AreaWeaponStatsModifier>();
        if (areaStats == null)
        {
            areaStats = player.gameObject.AddComponent<AreaWeaponStatsModifier>();
        }
        areaStats.ReduceCooldown(cooldownReduction);
    }
}
