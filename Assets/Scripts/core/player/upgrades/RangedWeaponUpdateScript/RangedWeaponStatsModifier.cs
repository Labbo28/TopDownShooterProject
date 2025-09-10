using UnityEngine;

public class RangedWeaponStatsModifier : MonoBehaviour
{
    private float damageMultiplier = 1f;
    private float fireRateMultiplier = 1f;
    private float reloadSpeedMultiplier = 1f;

    // Valori base delle armi ranged
    private struct RangedWeaponBaseStats {
        public float damage;
        public float fireRate;
        public float reloadTime;
    }
    private System.Collections.Generic.Dictionary<Weapon, RangedWeaponBaseStats> baseStats = new System.Collections.Generic.Dictionary<Weapon, RangedWeaponBaseStats>();

    public float DamageMultiplier => damageMultiplier;
    public float FireRateMultiplier => fireRateMultiplier;
    public float ReloadSpeedMultiplier => reloadSpeedMultiplier;

    private void Awake()
    {
        // Salva i valori base delle armi ranged
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && weapon.WeaponSo != null && !baseStats.ContainsKey(weapon))
            {
                float dmg = weapon.WeaponSo.projectile != null ? weapon.WeaponSo.projectile.damage : 0f;
                baseStats[weapon] = new RangedWeaponBaseStats
                {
                    damage = dmg,
                    fireRate = weapon.WeaponSo.fireRate,
                    reloadTime = weapon.WeaponSo.reloadTime
                };
            }
        }
    }

    public void AddDamageMultiplier(float multiplier)
    {
        damageMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddFireRateMultiplier(float multiplier)
    {
        fireRateMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddReloadSpeedMultiplier(float multiplier)
    {
        reloadSpeedMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    private void UpdateExistingWeapons()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && weapon.WeaponSo != null)
            {
                if (baseStats.ContainsKey(weapon))
                {
                    var stats = baseStats[weapon];
                    if (weapon.WeaponSo.projectile != null)
                        weapon.WeaponSo.projectile.damage = stats.damage * damageMultiplier;
                    weapon.WeaponSo.fireRate = stats.fireRate * fireRateMultiplier;
                    weapon.WeaponSo.reloadTime = stats.reloadTime * reloadSpeedMultiplier;
                }
            }
        }
    }

    public void ApplyToRangedWeapon(Weapon weapon)
    {
        if (weapon != null && !(weapon is SpinWeapon) && weapon.WeaponSo != null)
        {
            if (baseStats.ContainsKey(weapon))
            {
                var stats = baseStats[weapon];
                if (weapon.WeaponSo.projectile != null)
                    weapon.WeaponSo.projectile.damage = stats.damage * damageMultiplier;
                weapon.WeaponSo.fireRate = stats.fireRate * fireRateMultiplier;
                weapon.WeaponSo.reloadTime = stats.reloadTime * reloadSpeedMultiplier;
            }
            else
            {
                if (weapon.WeaponSo.projectile != null)
                    weapon.WeaponSo.projectile.damage *= damageMultiplier;
                weapon.WeaponSo.fireRate *= fireRateMultiplier;
                weapon.WeaponSo.reloadTime *= reloadSpeedMultiplier;
            }
        }
    }

    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        fireRateMultiplier = 1f;
        reloadSpeedMultiplier = 1f;

        // Ripristina i valori base su tutte le armi ranged
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && weapon.WeaponSo != null && baseStats.ContainsKey(weapon))
            {
                var stats = baseStats[weapon];
                if (weapon.WeaponSo.projectile != null)
                    weapon.WeaponSo.projectile.damage = stats.damage;
                weapon.WeaponSo.fireRate = stats.fireRate;
                weapon.WeaponSo.reloadTime = stats.reloadTime;
            }
        }
    }
}