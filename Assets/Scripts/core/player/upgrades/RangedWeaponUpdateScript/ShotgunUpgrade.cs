using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunUpgrade", menuName = "Upgrades/ShotgunUpgrade")]
public class ShotgunUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.2f; // +20% damage per level
    
    [Header("Fire Rate Settings")]
    [SerializeField] private float fireRateMultiplier = 0.85f; // Riduce il tempo tra colpi (aumenta velocità)
    
    [Header("Reload Settings")]
    [SerializeField] private float reloadSpeedMultiplier = 0.9f; // Riduce tempo di ricarica
    
    [Header("Magazine Settings")]
    [SerializeField] private float maxAmmoMultiplier = 1.3f; // +30% magazine size per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Unlock Shotgun on first upgrade
        if (currentLevel == 1)
        {
            WeaponUnlockManager unlockManager = player.GetComponent<WeaponUnlockManager>();
            if (unlockManager == null)
            {
                unlockManager = player.gameObject.AddComponent<WeaponUnlockManager>();
            }
            unlockManager.UnlockWeapon("Shotgun");
        }
        
        // Aggiungi o aggiorna il componente modificatore per armi ranged
        RangedWeaponStatsModifier rangedStats = player.GetComponent<RangedWeaponStatsModifier>();
        if (rangedStats == null)
        {
            rangedStats = player.gameObject.AddComponent<RangedWeaponStatsModifier>();
        }
        
        rangedStats.AddDamageMultiplier(damageMultiplier);
        rangedStats.AddFireRateMultiplier(fireRateMultiplier);
        rangedStats.AddReloadSpeedMultiplier(reloadSpeedMultiplier);
        rangedStats.AddMaxAmmoMultiplier(maxAmmoMultiplier);
    }
}