using UnityEngine;

[CreateAssetMenu(fileName = "SpinWeaponBladeUpgrade", menuName = "Upgrades/SpinWeaponBladeUpgrade")]
public class SpinWeaponBladeUpgrade : PlayerUpgrade
{
    [Header("Blade Count")]
    [SerializeField] private int additionalBlades = 1; // +1 blade per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        SpinWeaponStatsModifier spinStats = player.GetComponent<SpinWeaponStatsModifier>();
        if (spinStats == null)
        {
            spinStats = player.gameObject.AddComponent<SpinWeaponStatsModifier>();
        }
        spinStats.AddBladeCount(additionalBlades);
    }
}
