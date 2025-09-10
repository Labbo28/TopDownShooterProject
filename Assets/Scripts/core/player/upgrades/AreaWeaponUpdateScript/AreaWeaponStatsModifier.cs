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
    }

    public void AddRangeMultiplier(float multiplier)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.range *= multiplier;
        }
        rangeMultiplier *= multiplier;
    }

    public void ReduceCooldown(float reduction)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.cooldown = Mathf.Max(0.1f, areaWeapon.cooldown - reduction);
        }
        cooldownReduction += reduction;
    }

    public void AddDurationMultiplier(float multiplier)
    {
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            areaWeapon.duration *= multiplier;
        }
        durationMultiplier *= multiplier;
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

        }
    }

    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        rangeMultiplier = 1f;
        cooldownReduction = 0f;
        durationMultiplier = 1f;
    }

    public void GetModifiedStats(AreaWeapon baseWeapon, out float finalDamage, out float finalRange, out float finalCooldown, out float finalDuration)
    {
        finalDamage = baseWeapon.weaponDamage * damageMultiplier;
        finalRange = baseWeapon.range * rangeMultiplier;
        finalCooldown = Mathf.Max(0.1f, baseWeapon.cooldown - cooldownReduction);
        finalDuration = baseWeapon.duration * durationMultiplier;
    }
}