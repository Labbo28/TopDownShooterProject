using UnityEngine;

[CreateAssetMenu(fileName = "ThrowingWeaponUpgrade", menuName = "Upgrades/ThrowingWeapon/AllStats")]
public class ThrowingWeaponUpgrade : PlayerUpgrade
{
    [Header("Damage")]
    [SerializeField] private float damageMultiplier = 1.25f;
    
    [Header("Speed & Cooldown")]
    [SerializeField] private float speedMultiplier = 1.1f;
    [SerializeField] private float cooldownMultiplier = 0.9f; // Riduce cooldown = più veloce
    
    [Header("Range & Penetration")]
    [SerializeField] private float rangeMultiplier = 1.15f;
    [SerializeField] private int additionalPenetrations = 1;
    
    [Header("Detection")]
    [SerializeField] private float detectionRangeMultiplier = 1.2f;

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        Debug.Log($"ThrowingWeaponUpgrade.ApplyUpgrade called - Level: {currentLevel}");
        
        // Unlock ThrowingWeapon on first upgrade
        if (currentLevel == 0)
        {
            Debug.Log("Unlocking ThrowingWeapon for the first time");
            WeaponUnlockManager unlockManager = player.GetComponent<WeaponUnlockManager>();
            if (unlockManager == null)
            {
                Debug.Log("Creating new WeaponUnlockManager");
                unlockManager = player.gameObject.AddComponent<WeaponUnlockManager>();
            }
            unlockManager.UnlockWeapon("ThrowingWeapon");
        }
        else
        {
            Debug.Log($"Upgrading ThrowingWeapon to level {currentLevel + 1}");
        }

        // Gestisce tutto tramite ThrowingWeaponStatsModifier per evitare conflitti
        ThrowingWeaponStatsModifier throwingStats = player.GetComponent<ThrowingWeaponStatsModifier>();
        if (throwingStats == null)
        {
            throwingStats = player.gameObject.AddComponent<ThrowingWeaponStatsModifier>();
        }

        if (currentLevel != 0)
        {
            // Applica i modificatori (il modifier aggiorna le armi esistenti)
            throwingStats.AddDamageMultiplier(damageMultiplier);
            throwingStats.AddSpeedMultiplier(speedMultiplier);
            throwingStats.AddCooldownMultiplier(cooldownMultiplier);
            throwingStats.AddRangeMultiplier(rangeMultiplier);
            throwingStats.AddPenetrations(additionalPenetrations);
            throwingStats.AddDetectionRangeMultiplier(detectionRangeMultiplier);
            
            Debug.Log($"ThrowingWeapon upgraded - Level {currentLevel}: " +
                     $"Damage {damageMultiplier}x, Speed {speedMultiplier}x, " +
                     $"Cooldown {cooldownMultiplier}x, Range {rangeMultiplier}x, " +
                     $"Penetration +{additionalPenetrations}, Detection {detectionRangeMultiplier}x");
        }
    }
}