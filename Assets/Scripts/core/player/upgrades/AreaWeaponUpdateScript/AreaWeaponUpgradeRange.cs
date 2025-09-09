using UnityEngine;


 [CreateAssetMenu(fileName = "AreaWeaponUpgradeRange", menuName = "Upgrades/AreaWeaponUpgradeRange")]
public class AreaWeaponUpgradeRange : PlayerUpgrade
{



    [Header("Range Settings")]
    [SerializeField] private float rangeMultiplier = 1.1f; // +10% range per level

    // Applica l'upgrade tramite il modifier, come per le SpinWeapon
    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        // Gestisce tutto tramite AreaWeaponStatsModifier per evitare conflitti
        AreaWeaponStatsModifier areaStats = player.GetComponent<AreaWeaponStatsModifier>();
        if (areaStats == null)
        {
            areaStats = player.gameObject.AddComponent<AreaWeaponStatsModifier>();
        }

    // Applica solo il modificatore di range
    areaStats.AddRangeMultiplier(rangeMultiplier);

    Debug.Log($"Area Weapon Upgrade Range applied! Level: {currentLevel + 1}");
    Debug.Log($"Current stats - Range: x{areaStats.RangeMultiplier:F2}");
    }
}