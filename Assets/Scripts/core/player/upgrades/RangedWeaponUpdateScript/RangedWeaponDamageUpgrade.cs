using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponDamageUpgrade", menuName = "Upgrades/RangedWeaponDamageUpgrade")]
public class RangedWeaponDamageUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.2f; // +20% damage per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        RangedWeaponStatsModifier rangedStats = player.GetComponent<RangedWeaponStatsModifier>();
        if (rangedStats == null)
        {
            rangedStats = player.gameObject.AddComponent<RangedWeaponStatsModifier>();
        }
        rangedStats.AddDamageMultiplier(damageMultiplier);
    }
}
