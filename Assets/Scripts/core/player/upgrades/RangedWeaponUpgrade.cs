using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponUpgrade", menuName = "Upgrades/RangedWeaponUpgrade")]
public class RangedWeaponUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.2f; // +20% damage per level
    
    [Header("Fire Rate Settings")]
    [SerializeField] private float fireRateMultiplier = 0.85f; // Riduce il tempo tra colpi (aumenta velocità)
    
    [Header("Reload Settings")]
    [SerializeField] private float reloadSpeedMultiplier = 0.9f; // Riduce tempo di ricarica

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Aggiungi o aggiorna il componente modificatore per armi ranged
        RangedWeaponStatsModifier rangedStats = player.GetComponent<RangedWeaponStatsModifier>();
        if (rangedStats == null)
        {
            rangedStats = player.gameObject.AddComponent<RangedWeaponStatsModifier>();
        }
        
        rangedStats.AddDamageMultiplier(damageMultiplier);
        rangedStats.AddFireRateMultiplier(fireRateMultiplier);
        rangedStats.AddReloadSpeedMultiplier(reloadSpeedMultiplier);
        
        Debug.Log($"Ranged Weapon Upgrade applied! Level: {currentLevel + 1}");
        Debug.Log($"Current multipliers - Damage: {rangedStats.DamageMultiplier}, FireRate: {rangedStats.FireRateMultiplier}, Reload: {rangedStats.ReloadSpeedMultiplier}");
    }
}