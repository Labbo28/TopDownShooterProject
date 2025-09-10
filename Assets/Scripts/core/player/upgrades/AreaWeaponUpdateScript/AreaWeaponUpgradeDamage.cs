using UnityEngine;

[CreateAssetMenu(fileName = "AreaWeaponUpgradeDamage", menuName = "Upgrades/AreaWeaponUpgradeDamage")]
public class AreaWeaponUpgradeDamage : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.25f; // +25% damage per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        AreaWeaponStatsModifier areaStats = player.GetComponent<AreaWeaponStatsModifier>();
        if (areaStats == null)
        {
            areaStats = player.gameObject.AddComponent<AreaWeaponStatsModifier>();
        }
        areaStats.AddDamageMultiplier(damageMultiplier);
    }
}
