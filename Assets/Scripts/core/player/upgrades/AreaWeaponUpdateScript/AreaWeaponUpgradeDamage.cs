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
        Debug.Log($"Area Weapon Upgrade Damage applied! Level: {currentLevel + 1}");
        Debug.Log($"Current stats - Damage: x{areaStats.DamageMultiplier:F2}");
    }
}
