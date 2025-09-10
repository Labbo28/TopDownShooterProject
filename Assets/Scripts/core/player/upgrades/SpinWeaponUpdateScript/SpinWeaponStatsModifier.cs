using UnityEngine;

public class SpinWeaponStatsModifier : MonoBehaviour
{
    private int additionalBladeCount = 0;
    private float damageMultiplier = 1f;
    private float speedMultiplier = 1f;
    private float rangeMultiplier = 1f;

    // Valori base delle armi spin
    private struct SpinWeaponBaseStats {
        public int amount;
        public float weaponDamage;
        public float speed;
        public float range;
    }
    private System.Collections.Generic.Dictionary<SpinWeapon, SpinWeaponBaseStats> baseStats = new System.Collections.Generic.Dictionary<SpinWeapon, SpinWeaponBaseStats>();

    public int AdditionalBladeCount => additionalBladeCount;
    public float DamageMultiplier => damageMultiplier;
    public float SpeedMultiplier => speedMultiplier;

    private void Awake()
    {
        // Salva i valori base delle armi spin
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            if (!baseStats.ContainsKey(spinWeapon))
            {
                baseStats[spinWeapon] = new SpinWeaponBaseStats
                {
                    amount = spinWeapon.amount,
                    weaponDamage = spinWeapon.weaponDamage,
                    speed = spinWeapon.speed,
                    range = spinWeapon.range
                };
            }
        }
    }

    public void AddBladeCount(int count)
    {
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            spinWeapon.amount += count;
        }
        additionalBladeCount += count;
    }

    public void AddDamageMultiplier(float multiplier)
    {
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            spinWeapon.weaponDamage *= multiplier;
        }
        damageMultiplier *= multiplier;
    }

    public void AddSpeedMultiplier(float multiplier)
    {
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            spinWeapon.speed *= multiplier;
        }
        speedMultiplier *= multiplier;
    }

    public void ApplyToNewSpinWeapon(SpinWeapon spinWeapon)
    {
        if (spinWeapon != null)
        {
            if (baseStats.ContainsKey(spinWeapon))
            {
                // Ripristina i valori base prima di applicare i modificatori
                var stats = baseStats[spinWeapon];
                spinWeapon.amount = stats.amount + additionalBladeCount;
                spinWeapon.weaponDamage = stats.weaponDamage * damageMultiplier;
                spinWeapon.speed = stats.speed * speedMultiplier;
                spinWeapon.range = stats.range * rangeMultiplier;
            }
            else
            {
                spinWeapon.amount += additionalBladeCount;
                spinWeapon.weaponDamage *= damageMultiplier;
                spinWeapon.speed *= speedMultiplier;
                spinWeapon.range *= rangeMultiplier;
            }
        }
    }

    public void ResetModifiers()
    {
        additionalBladeCount = 0;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        rangeMultiplier = 1f;

        // Ripristina i valori base su tutte le SpinWeapon
        SpinWeapon[] spinWeapons = GetComponentsInChildren<SpinWeapon>();
        foreach (SpinWeapon spinWeapon in spinWeapons)
        {
            if (baseStats.ContainsKey(spinWeapon))
            {
                var stats = baseStats[spinWeapon];
                spinWeapon.amount = stats.amount;
                spinWeapon.weaponDamage = stats.weaponDamage;
                spinWeapon.speed = stats.speed;
                spinWeapon.range = stats.range;
            }
        }
    }

    public void GetModifiedStats(SpinWeapon baseWeapon, out int finalBladeCount, out float finalDamage, out float finalSpeed, out float finalRange)
    {
        if (baseStats.ContainsKey(baseWeapon))
        {
            var stats = baseStats[baseWeapon];
            finalBladeCount = stats.amount + additionalBladeCount;
            finalDamage = stats.weaponDamage * damageMultiplier;
            finalSpeed = stats.speed * speedMultiplier;
            finalRange = stats.range * rangeMultiplier;
        }
        else
        {
            finalBladeCount = baseWeapon.amount + additionalBladeCount;
            finalDamage = baseWeapon.weaponDamage * damageMultiplier;
            finalSpeed = baseWeapon.speed * speedMultiplier;
            finalRange = baseWeapon.range * rangeMultiplier;
        }
    }
}