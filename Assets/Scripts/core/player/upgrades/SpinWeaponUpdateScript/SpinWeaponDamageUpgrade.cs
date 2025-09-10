using UnityEngine;

[CreateAssetMenu(fileName = "SpinWeaponDamageUpgrade", menuName = "Upgrades/SpinWeaponDamageUpgrade")]
public class SpinWeaponDamageUpgrade : PlayerUpgrade
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1.25f; // +25% damage per level

    public override void ApplyUpgrade(Player player, int currentLevel)
    {
        SpinWeaponStatsModifier spinStats = player.GetComponent<SpinWeaponStatsModifier>();
        if (spinStats == null)
        {
            spinStats = player.gameObject.AddComponent<SpinWeaponStatsModifier>();
        }
        spinStats.AddDamageMultiplier(damageMultiplier);
    }
}
