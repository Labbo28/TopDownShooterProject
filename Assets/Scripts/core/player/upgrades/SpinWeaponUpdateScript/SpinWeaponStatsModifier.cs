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
            Debug.Log($"Added {count} blades to {spinWeapon.name}: {oldAmount} -> {spinWeapon.amount}");
        }
        
        additionalBladeCount += count;
        Debug.Log($"Total additional blade count: {additionalBladeCount}");
    }

    public void AddDamageMultiplier(float multiplier)
    {
        // Applica il moltiplicatore di danno alle armi esistenti
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            float oldDamage = spinWeapon.weaponDamage;
            spinWeapon.weaponDamage *= multiplier;
            Debug.Log($"Applied damage multiplier {multiplier:F2} to {spinWeapon.name}: {oldDamage:F1} -> {spinWeapon.weaponDamage:F1}");
        }
        
        damageMultiplier *= multiplier;
        Debug.Log($"Total spin weapon damage multiplier: {damageMultiplier:F2}");
    }

    public void AddSpeedMultiplier(float multiplier)
    {
        // Applica il moltiplicatore di velocità alle armi esistenti
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            float oldSpeed = spinWeapon.speed;
            spinWeapon.speed *= multiplier;
            Debug.Log($"Applied speed multiplier {multiplier:F2} to {spinWeapon.name}: {oldSpeed:F2} -> {spinWeapon.speed:F2}");
        }
        
        speedMultiplier *= multiplier;
        Debug.Log($"Total spin weapon speed multiplier: {speedMultiplier:F2}");
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
            
            Debug.Log($"Applied all spin weapon modifiers to new weapon {spinWeapon.name}:");
            Debug.Log($"  - Blades: +{additionalBladeCount} (total: {spinWeapon.amount})");
            Debug.Log($"  - Damage: x{damageMultiplier:F2} (total: {spinWeapon.weaponDamage:F1})");
            Debug.Log($"  - Speed: x{speedMultiplier:F2} (total: {spinWeapon.speed:F2})");
            Debug.Log($"  - Range: x{rangeMultiplier:F2} (total: {spinWeapon.range:F2})");
        }
    }

    // Resetta tutti i modificatori (utile per il reset del gioco)
    public void ResetModifiers()
    {
        additionalBladeCount = 0;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        rangeMultiplier = 1f;
        Debug.Log("Spin weapon modifiers reset");
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