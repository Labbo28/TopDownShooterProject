using UnityEngine;

public class AreaWeaponStatsModifier : MonoBehaviour
{
    private float damageMultiplier = 1f;
    private float rangeMultiplier = 1f;
    private float cooldownReduction = 0f;
    private float durationMultiplier = 1f;

    // Valori base delle armi area
    private struct AreaWeaponBaseStats {
        public float weaponDamage;
        public float range;
        public float cooldown;
        public float duration;
    }
    private System.Collections.Generic.Dictionary<AreaWeapon, AreaWeaponBaseStats> baseStats = new System.Collections.Generic.Dictionary<AreaWeapon, AreaWeaponBaseStats>();

    public float DamageMultiplier => damageMultiplier;
    public float RangeMultiplier => rangeMultiplier;
    public float Cooldown => cooldownReduction;
    public float DurationMultiplier => durationMultiplier;

    private void Awake()
    {
        // Salva i valori base delle armi area
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            if (!baseStats.ContainsKey(areaWeapon))
            {
                baseStats[areaWeapon] = new AreaWeaponBaseStats
                {
                    weaponDamage = areaWeapon.weaponDamage,
                    range = areaWeapon.range,
                    cooldown = areaWeapon.cooldown,
                    duration = areaWeapon.duration
                };
            }
        }
    }

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

    public void ApplyToNewAreaWeapon(AreaWeapon areaWeapon)
    {
        if (areaWeapon != null)
        {
            if (baseStats.ContainsKey(areaWeapon))
            {
                // Ripristina i valori base prima di applicare i modificatori
                var stats = baseStats[areaWeapon];
                areaWeapon.weaponDamage = stats.weaponDamage * damageMultiplier;
                areaWeapon.range = stats.range * rangeMultiplier;
                areaWeapon.cooldown = Mathf.Max(0.1f, stats.cooldown - cooldownReduction);
                areaWeapon.duration = stats.duration * durationMultiplier;
            }
            else
            {
                areaWeapon.weaponDamage *= damageMultiplier;
                areaWeapon.range *= rangeMultiplier;
                areaWeapon.cooldown = Mathf.Max(0.1f, areaWeapon.cooldown - cooldownReduction);
                areaWeapon.duration *= durationMultiplier;
            }
        }
    }

    public void ResetModifiers()
    {
        damageMultiplier = 1f;
        rangeMultiplier = 1f;
        cooldownReduction = 0f;
        durationMultiplier = 1f;

        // Ripristina i valori base su tutte le AreaWeapon
        AreaWeapon[] areaWeapons = GetComponentsInChildren<AreaWeapon>();
        foreach (AreaWeapon areaWeapon in areaWeapons)
        {
            if (baseStats.ContainsKey(areaWeapon))
            {
                var stats = baseStats[areaWeapon];
                areaWeapon.weaponDamage = stats.weaponDamage;
                areaWeapon.range = stats.range;
                areaWeapon.cooldown = stats.cooldown;
                areaWeapon.duration = stats.duration;
            }
        }
    }

    public void GetModifiedStats(AreaWeapon baseWeapon, out float finalDamage, out float finalRange, out float finalCooldown, out float finalDuration)
    {
        if (baseStats.ContainsKey(baseWeapon))
        {
            var stats = baseStats[baseWeapon];
            finalDamage = stats.weaponDamage * damageMultiplier;
            finalRange = stats.range * rangeMultiplier;
            finalCooldown = Mathf.Max(0.1f, stats.cooldown - cooldownReduction);
            finalDuration = stats.duration * durationMultiplier;
        }
        else
        {
            finalDamage = baseWeapon.weaponDamage * damageMultiplier;
            finalRange = baseWeapon.range * rangeMultiplier;
            finalCooldown = Mathf.Max(0.1f, baseWeapon.cooldown - cooldownReduction);
            finalDuration = baseWeapon.duration * durationMultiplier;
        }
    }
}