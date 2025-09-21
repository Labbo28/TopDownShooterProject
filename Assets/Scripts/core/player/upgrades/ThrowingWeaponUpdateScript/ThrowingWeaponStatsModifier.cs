using UnityEngine;

/// <summary>
/// Gestisce i modificatori delle statistiche per ThrowingWeapon
/// </summary>
public class ThrowingWeaponStatsModifier : MonoBehaviour
{
    [Header("Damage Modifiers")]
    private float damageMultiplier = 1f;
    
    [Header("Speed & Cooldown Modifiers")]
    private float speedMultiplier = 1f;
    private float cooldownMultiplier = 1f;
    
    [Header("Range & Penetration Modifiers")]
    private float rangeMultiplier = 1f;
    private int additionalPenetrations = 0;
    
    [Header("Detection Modifiers")]
    private float detectionRangeMultiplier = 1f;

    // Properties per accesso esterno
    public float DamageMultiplier => damageMultiplier;
    public float SpeedMultiplier => speedMultiplier;
    public float CooldownMultiplier => cooldownMultiplier;
    public float RangeMultiplier => rangeMultiplier;
    public int AdditionalPenetrations => additionalPenetrations;
    public float DetectionRangeMultiplier => detectionRangeMultiplier;

    // Metodi per aggiungere modificatori
    public void AddDamageMultiplier(float multiplier)
    {
        damageMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddSpeedMultiplier(float multiplier)
    {
        speedMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddCooldownMultiplier(float multiplier)
    {
        cooldownMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddRangeMultiplier(float multiplier)
    {
        rangeMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    public void AddPenetrations(int additional)
    {
        additionalPenetrations += additional;
        UpdateExistingWeapons();
    }

    public void AddDetectionRangeMultiplier(float multiplier)
    {
        detectionRangeMultiplier *= multiplier;
        UpdateExistingWeapons();
    }

    /// <summary>
    /// Aggiorna tutte le ThrowingWeapon esistenti con i nuovi modificatori
    /// </summary>
    private void UpdateExistingWeapons()
    {
        ThrowingWeapon[] throwingWeapons = FindObjectsOfType<ThrowingWeapon>();
        foreach (ThrowingWeapon weapon in throwingWeapons)
        {
            weapon.ApplyStatsModifier(this);
        }
    }

    /// <summary>
    /// Reset di tutti i modificatori (utile per testing)
    /// </summary>
    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        cooldownMultiplier = 1f;
        rangeMultiplier = 1f;
        additionalPenetrations = 0;
        detectionRangeMultiplier = 1f;
        UpdateExistingWeapons();
    }
}