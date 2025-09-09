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
        Debug.Log($"Spin Weapon Damage Upgrade applied! Level: {currentLevel + 1}");
        Debug.Log($"Current Damage multiplier: {spinStats.DamageMultiplier}");
    }
}
