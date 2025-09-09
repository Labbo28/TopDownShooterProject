using UnityEngine;

public class AreaWeaponStatsModifier : MonoBehaviour
{
    private float damageMultiplier = 1f;
    private float rangeMultiplier = 1f;
    private float cooldownReduction = 0f;
    private float durationMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float RangeMultiplier => rangeMultiplier;
    public float Cooldown => cooldownReduction;
    public float DurationMultiplier => durationMultiplier;

    public void AddDamageMultiplier(float multiplier)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.weaponDamage *= multiplier;
        }
        damageMultiplier *= multiplier;
        Debug.Log($"Total area weapon damage multiplier: {damageMultiplier:F2}");
    }

    public void AddRangeMultiplier(float multiplier)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.range *= multiplier;
        }
        rangeMultiplier *= multiplier;
        Debug.Log($"Total area weapon range multiplier: {rangeMultiplier:F2}");
    }

    public void ReduceCooldown(float reduction)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.cooldown = Mathf.Max(0.1f, areaWeapon.cooldown - reduction);
        }
        cooldownReduction += reduction;
        Debug.Log($"Total area weapon cooldown reduction: -{cooldownReduction:F2}");
    }

    public void AddDurationMultiplier(float multiplier)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.duration *= multiplier;
        }
        durationMultiplier *= multiplier;
        Debug.Log($"Total area weapon duration multiplier: {durationMultiplier:F2}");
    }

    // Apply all modifiers to a new AreaWeapon
    public void ApplyToNewAreaWeapon(AreaWeapon areaWeapon)
    {
        if (areaWeapon != null)
        {
            areaWeapon.weaponDamage *= damageMultiplier;
            areaWeapon.range *= rangeMultiplier;
            areaWeapon.cooldown = Mathf.Max(0.1f, areaWeapon.cooldown - cooldownReduction);
            areaWeapon.duration *= durationMultiplier;

            Debug.Log($"Applied all area weapon modifiers to new weapon {areaWeapon.name}:");
            Debug.Log($"  - Damage: x{damageMultiplier:F2} (total: {areaWeapon.weaponDamage:F1})");
            Debug.Log($"  - Range: x{rangeMultiplier:F2} (total: {areaWeapon.range:F2})");
            Debug.Log($"  - Cooldown: -{cooldownReduction:F2} (total: {areaWeapon.cooldown:F2})");
            Debug.Log($"  - Duration: x{durationMultiplier:F2} (total: {areaWeapon.duration:F2})");
        }
    }

    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        rangeMultiplier = 1f;
        cooldownReduction = 0f;
        durationMultiplier = 1f;
        Debug.Log("Area weapon modifiers reset");
    }

    public void GetModifiedStats(AreaWeapon baseWeapon, out float finalDamage, out float finalRange, out float finalCooldown, out float finalDuration)
    {
        finalDamage = baseWeapon.weaponDamage * damageMultiplier;
        finalRange = baseWeapon.range * rangeMultiplier;
        finalCooldown = Mathf.Max(0.1f, baseWeapon.cooldown - cooldownReduction);
        finalDuration = baseWeapon.duration * durationMultiplier;
    }
}