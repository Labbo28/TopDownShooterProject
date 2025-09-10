using UnityEngine;

public class RangedWeaponStatsModifier : MonoBehaviour
{
    private float damageMultiplier = 1f;
    private float fireRateMultiplier = 1f;
    private float reloadSpeedMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float FireRateMultiplier => fireRateMultiplier;
    public float ReloadSpeedMultiplier => reloadSpeedMultiplier;

    public void AddDamageMultiplier(float multiplier)
    {
        damageMultiplier *= multiplier;
    }

    public void AddFireRateMultiplier(float multiplier)
    {
        fireRateMultiplier *= multiplier;
        
        // Aggiorna le armi esistenti
        UpdateExistingWeapons();
    }

    public void AddReloadSpeedMultiplier(float multiplier)
    {
        reloadSpeedMultiplier *= multiplier;
        
        // Aggiorna le armi esistenti
        UpdateExistingWeapons();
    }
    
    private void UpdateExistingWeapons()
    {
        // Trova tutte le armi ranged e aggiorna i loro timer
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (!(weapon is SpinWeapon) && weapon.WeaponSo != null)
            {
                // Chiama il metodo di aggiornamento se esiste
                var updateMethod = weapon.GetType().GetMethod("UpdateTimersWithModifiers", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                updateMethod?.Invoke(weapon, null);
            }
        }
    }

    // Metodo per applicare i modificatori a una nuova arma ranged
    public void ApplyToRangedWeapon(Weapon weapon)
    {
        if (weapon != null && !(weapon is SpinWeapon) && weapon.WeaponSo != null)
        {
            // Applica damage
            if (weapon.WeaponSo.projectile != null)
            {
                weapon.WeaponSo.projectile.damage *= damageMultiplier;
            }
            
            // Applica fire rate
            weapon.WeaponSo.fireRate *= fireRateMultiplier;
            
            // Applica reload speed
            weapon.WeaponSo.reloadTime *= reloadSpeedMultiplier;
            
        }
    }

    // Resetta tutti i modificatori (utile per il reset del gioco)
    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        fireRateMultiplier = 1f;
        reloadSpeedMultiplier = 1f;
    }
}