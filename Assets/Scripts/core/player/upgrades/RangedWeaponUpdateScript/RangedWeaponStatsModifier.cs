using UnityEngine;
using System.Collections.Generic;

public class RangedWeaponStatsModifier : MonoBehaviour
{
    private float fireRateMultiplier = 1f;
    private float reloadSpeedMultiplier = 1f;
    private float maxAmmoMultiplier = 1f;

    public float DamageMultiplier { get; private set; } = 1f; // serve solo per i Projectile

    private struct RangedWeaponBaseStats
    {
        public float fireRate;
        public float reloadTime;
        public int maxAmmo;
    }

    private Dictionary<Weapon, RangedWeaponBaseStats> baseStats =
        new Dictionary<Weapon, RangedWeaponBaseStats>();

    public float FireRateMultiplier => fireRateMultiplier;
    public float ReloadSpeedMultiplier => reloadSpeedMultiplier;
    public float MaxAmmoMultiplier => maxAmmoMultiplier;

    private void Awake()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && !baseStats.ContainsKey(weapon))
            {
                baseStats[weapon] = new RangedWeaponBaseStats
                {
                    fireRate = weapon.FireRate,
                    reloadTime = weapon.ReloadTime,
                    maxAmmo = weapon.MaxAmmo
                };
            }
        }
    }

    public void AddDamageMultiplier(float multiplier)
    {
        DamageMultiplier *= multiplier;
        // Non serve aggiornare Weapon, il danno viene letto dal Projectile
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

    public void AddMaxAmmoMultiplier(float multiplier)
    {
        maxAmmoMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    private void UpdateExistingWeapons()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && baseStats.ContainsKey(weapon))
            {
                var stats = baseStats[weapon];

                weapon.FireRate = stats.fireRate * fireRateMultiplier;
                weapon.ReloadTime = stats.reloadTime * reloadSpeedMultiplier;
                weapon.MaxAmmo = Mathf.RoundToInt(stats.maxAmmo * maxAmmoMultiplier);

                weapon.UpdateTimersWithModifiers();
            }
        }
    }

    public void ResetModifiers()
    {
        DamageMultiplier = 1f;
        fireRateMultiplier = 1f;
        reloadSpeedMultiplier = 1f;
        maxAmmoMultiplier = 1f;

        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && baseStats.ContainsKey(weapon))
            {
                var stats = baseStats[weapon];

                weapon.FireRate = stats.fireRate;
                weapon.ReloadTime = stats.reloadTime;
                weapon.MaxAmmo = stats.maxAmmo;
            }
        }
    }
}
