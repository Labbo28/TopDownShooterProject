using UnityEngine;

[CreateAssetMenu(fileName = "SpinWeaponUpgrade", menuName = "Upgrades/SpinWeaponUpgrade")]
public class SpinWeaponUpgrade : PlayerUpgrade
{
    [Header("Blade Count")]
    [SerializeField] private int additionalBlades = 1; // +1 blade per level
    
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.25f; // +25% damage per level
    
    [Header("Speed Settings")]
    [SerializeField] private float speedMultiplier = 1.03f; // +3% rotation speed per level
    


    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Gestisce tutto tramite SpinWeaponStatsModifier per evitare conflitti
        SpinWeaponStatsModifier spinStats = player.GetComponent<SpinWeaponStatsModifier>();
        if (spinStats == null)
        {
            spinStats = player.gameObject.AddComponent<SpinWeaponStatsModifier>();
        }
        
        // Applica i modificatori (il modificatore si occuperà di aggiornare le armi esistenti)
        spinStats.AddBladeCount(additionalBlades);
        spinStats.AddDamageMultiplier(damageMultiplier);
        spinStats.AddSpeedMultiplier(speedMultiplier);
    
        
    }
}