using UnityEngine;

public class SpinWeaponStatsModifier : MonoBehaviour
{
    private int additionalBladeCount = 0;
    private float damageMultiplier = 1f;
    private float speedMultiplier = 1f;
    private float rangeMultiplier = 1f;

    public int AdditionalBladeCount => additionalBladeCount;
    public float DamageMultiplier => damageMultiplier;
    public float SpeedMultiplier => speedMultiplier;
   

    public void AddBladeCount(int count)
    {
        // Applica l'incremento di lame alle armi esistenti prima di aggiornare il totale
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            int oldAmount = spinWeapon.amount;
            spinWeapon.amount += count;
        }
        
        additionalBladeCount += count;
    }

    public void AddDamageMultiplier(float multiplier)
    {
        // Applica il moltiplicatore di danno alle armi esistenti
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            float oldDamage = spinWeapon.weaponDamage;
            spinWeapon.weaponDamage *= multiplier;
        }
        
        damageMultiplier *= multiplier;
    }

    public void AddSpeedMultiplier(float multiplier)
    {
        // Applica il moltiplicatore di velocità alle armi esistenti
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            float oldSpeed = spinWeapon.speed;
            spinWeapon.speed *= multiplier;
        }
        
        speedMultiplier *= multiplier;
    }



    // Metodo per applicare i modificatori a una nuova SpinWeapon
    public void ApplyToNewSpinWeapon(SpinWeapon spinWeapon)
    {
        if (spinWeapon != null)
        {
            // Applica tutti i modificatori accumulati a una nuova arma
            spinWeapon.amount += additionalBladeCount;
            spinWeapon.weaponDamage *= damageMultiplier;
            spinWeapon.speed *= speedMultiplier;
            spinWeapon.range *= rangeMultiplier;
            
        }
    }

    // Resetta tutti i modificatori (utile per il reset del gioco)
    public void ResetModifiers()
    {
        additionalBladeCount = 0;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        rangeMultiplier = 1f;
    }

    // Metodo helper per ottenere i valori base di una SpinWeapon modificata
    public void GetModifiedStats(SpinWeapon baseWeapon, out int finalBladeCount, out float finalDamage, out float finalSpeed, out float finalRange)
    {
        finalBladeCount = baseWeapon.amount + additionalBladeCount;
        finalDamage = baseWeapon.weaponDamage * damageMultiplier;
        finalSpeed = baseWeapon.speed * speedMultiplier;
        finalRange = baseWeapon.range * rangeMultiplier;
    }
}