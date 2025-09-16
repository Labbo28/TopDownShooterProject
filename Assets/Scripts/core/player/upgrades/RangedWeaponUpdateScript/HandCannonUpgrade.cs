using UnityEngine;

[CreateAssetMenu(fileName = "HandCannonUpgrade", menuName = "Upgrades/HandCannonUpgrade")]
public class HandCannonUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.4f; // +40% damage per level (HandCannon is high-damage, low-fire rate)
    
    [Header("Fire Rate Settings")]
    [SerializeField] private float fireRateMultiplier = 0.9f; // Slight fire rate improvement
    
    [Header("Reload Settings")]
    [SerializeField] private float reloadSpeedMultiplier = 0.85f; // Faster reload to compensate for slower fire rate
    
    [Header("Magazine Settings")]
    [SerializeField] private float maxAmmoMultiplier = 1.2f; // +20% magazine size per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Unlock HandCannon on first upgrade
        if (currentLevel == 1)
        {
            WeaponUnlockManager unlockManager = player.GetComponent<WeaponUnlockManager>();
            if (unlockManager == null)
            {
                unlockManager = player.gameObject.AddComponent<WeaponUnlockManager>();
            }
            unlockManager.UnlockWeapon("HandCannon");
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