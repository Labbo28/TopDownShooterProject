using UnityEngine;


 [CreateAssetMenu(fileName = "AreaWeaponUpgrade", menuName = "Upgrades/AreaWeaponUpgrade")]
public class AreaWeaponUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.25f; // +25% damage per level

    [Header("Range Settings")]
    [SerializeField] private float rangeMultiplier = 1.1f; // +10% range per level

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownReduction = 0.5f; // -0.5 seconds cooldown per level

    [Header("Duration Settings")]
    [SerializeField] private float durationMultiplier = 1.1f; // +10% duration per level

    // Applica l'upgrade tramite il modifier, come per le SpinWeapon
    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Unlock AreaWeapon on first upgrade
        if (currentLevel == 1)
        {
            WeaponUnlockManager unlockManager = player.GetComponent<WeaponUnlockManager>();
            if (unlockManager == null)
            {
                unlockManager = player.gameObject.AddComponent<WeaponUnlockManager>();
            }
            unlockManager.UnlockWeapon("AreaWeapon");
        }
        
            // Gestisce tutto tramite AreaWeaponStatsModifier per evitare conflitti
            AreaWeaponStatsModifier areaStats = player.GetComponent<AreaWeaponStatsModifier>();
            if (areaStats == null)
            {
                areaStats = player.gameObject.AddComponent<AreaWeaponStatsModifier>();
            }
            if(currentLevel != 0)
            {
                 // Applica i modificatori (il modifier aggiorna le armi esistenti)
            areaStats.AddDamageMultiplier(damageMultiplier);
            areaStats.AddRangeMultiplier(rangeMultiplier);
            areaStats.ReduceCooldown(cooldownReduction);
            areaStats.AddDurationMultiplier(durationMultiplier);
        
            }
           

    }
}