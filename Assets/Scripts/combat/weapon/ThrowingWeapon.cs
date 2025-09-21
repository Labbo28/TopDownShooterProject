using UnityEngine;

/// <summary>
/// ThrowingWeapon - Arma che lancia automaticamente proiettili verso il nemico più vicino
/// I proiettili penetrano nemici, raggiungono max range, poi tornano indietro
/// </summary>
public class ThrowingWeapon : Weapon
{
    [Header("Throwing Weapon Settings")]
    [SerializeField] private GameObject throwingProjectilePrefab;
    [SerializeField] public float weaponDamage = 25f;
    [SerializeField] public float throwSpeed = 8f;
    [SerializeField] public float maxThrowRange = 4f;
    [SerializeField] public float throwCooldown = 2f;
    [SerializeField] public int maxPenetrations = 3;
    [SerializeField] private float enemyDetectionRange = 8f;
    
    private float lastThrowTime = 0f;
    private Transform player;

    void Start()
    {
        player = FindObjectOfType<Player>()?.transform;
        if (player == null)
        {
            Debug.LogError("Player non trovato! ThrowingWeapon non può funzionare.");
        }
    }

    void Update()
    {
        if (player == null || !IsPlayerAlive()) return;
        
        // Lancio automatico basato su cooldown
        if (Time.time >= lastThrowTime + GetFinalCooldown())
        {
            Transform nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                ThrowProjectile(nearestEnemy.position);
                lastThrowTime = Time.time;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float nearestDistance = enemyDetectionRange;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    private void ThrowProjectile(Vector3 targetPosition)
    {
        if (throwingProjectilePrefab == null)
        {
            Debug.LogError("throwingProjectilePrefab non assegnato!");
            return;
        }

        // Spawn projectile alla posizione del player
        GameObject projectile = Instantiate(throwingProjectilePrefab, player.position, Quaternion.identity);
        
        // Configura il projectile
        ThrowingProjectile throwingScript = projectile.GetComponent<ThrowingProjectile>();
        if (throwingScript != null)
        {
            // Direzione verso il target
            Vector3 direction = (targetPosition - player.position).normalized;
            
            // Applica upgrades
            throwingScript.SetDamage(GetFinalDamage());
            throwingScript.SetSpeed(GetFinalSpeed());
            throwingScript.SetMaxRange(GetFinalRange());
            throwingScript.SetMaxPenetrations(GetFinalPenetrations());
            
            // Inizializza con direzione
            throwingScript.Initialize(direction);
        }
        
        // Audio feedback
        AudioManager.Instance?.PlayRangedSound(); // Riusa il suono ranged o crea uno specifico
    }

    private bool IsPlayerAlive()
    {
        if (Player.Instance == null) return false;
        HealthSystem healthSystem = Player.Instance.GetComponent<HealthSystem>();
        return healthSystem != null && healthSystem.IsAlive;
    }

    // ===== CALCOLI FINALI CON UPGRADES =====
    
    private float GetFinalDamage()
    {
        float finalDamage = weaponDamage;
        
        // Applica modificatori ThrowingWeapon
        if (Player.Instance != null)
        {
            ThrowingWeaponStatsModifier modifier = Player.Instance.GetComponent<ThrowingWeaponStatsModifier>();
            if (modifier != null)
            {
                finalDamage *= modifier.DamageMultiplier;
            }
        }
        
        return finalDamage;
    }

    private float GetFinalSpeed()
    {
        float finalSpeed = throwSpeed;
        
        if (Player.Instance != null)
        {
            ThrowingWeaponStatsModifier modifier = Player.Instance.GetComponent<ThrowingWeaponStatsModifier>();
            if (modifier != null)
            {
                finalSpeed *= modifier.SpeedMultiplier;
            }
        }
        
        return finalSpeed;
    }

    private float GetFinalRange()
    {
        float finalRange = maxThrowRange;
        
        if (Player.Instance != null)
        {
            ThrowingWeaponStatsModifier modifier = Player.Instance.GetComponent<ThrowingWeaponStatsModifier>();
            if (modifier != null)
            {
                finalRange *= modifier.RangeMultiplier;
            }
        }
        
        return finalRange;
    }

    private int GetFinalPenetrations()
    {
        int finalPenetrations = maxPenetrations;
        
        if (Player.Instance != null)
        {
            ThrowingWeaponStatsModifier modifier = Player.Instance.GetComponent<ThrowingWeaponStatsModifier>();
            if (modifier != null)
            {
                finalPenetrations += modifier.AdditionalPenetrations;
            }
        }
        
        return finalPenetrations;
    }

    private float GetFinalCooldown()
    {
        float finalCooldown = throwCooldown;
        
        if (Player.Instance != null)
        {
            ThrowingWeaponStatsModifier modifier = Player.Instance.GetComponent<ThrowingWeaponStatsModifier>();
            if (modifier != null)
            {
                finalCooldown *= modifier.CooldownMultiplier;
            }
        }
        
        return finalCooldown;
    }

    /// <summary>
    /// Chiamato dal ThrowingWeaponStatsModifier quando i modificatori cambiano
    /// </summary>
    public void ApplyStatsModifier(ThrowingWeaponStatsModifier modifier)
    {
        // I calcoli vengono fatti nei metodi GetFinal...() 
        // Non serve aggiornare nulla qui, i valori sono calcolati dinamicamente
        
        // Aggiorna detection range
        if (modifier != null)
        {
            enemyDetectionRange = 8f * modifier.DetectionRangeMultiplier; // 8f è il valore base
        }
    }

    // ===== METODI PUBBLICI PER UPGRADES =====
    
    public void UpgradeDamage(float multiplier)
    {
        weaponDamage *= multiplier;
    }

    public void UpgradeSpeed(float multiplier)
    {
        throwSpeed *= multiplier;
    }

    public void UpgradeRange(float multiplier)
    {
        maxThrowRange *= multiplier;
    }

    public void UpgradeCooldown(float multiplier)
    {
        throwCooldown *= multiplier; // Moltiplicatore < 1 per cooldown più veloce
    }

    public void UpgradePenetrations(int additionalPenetrations)
    {
        maxPenetrations += additionalPenetrations;
    }

    public void UpgradeDetectionRange(float multiplier)
    {
        enemyDetectionRange *= multiplier;
    }

    // ===== GETTERS PER DEBUG/UI =====
    
    public float GetCurrentDamage() => weaponDamage;
    public float GetCurrentSpeed() => throwSpeed;
    public float GetCurrentRange() => maxThrowRange;
    public float GetCurrentCooldown() => throwCooldown;
    public int GetCurrentPenetrations() => maxPenetrations;
    public float GetDetectionRange() => enemyDetectionRange;

    // Override obbligatorio dalla classe base Weapon
    protected override void Shoot()
    {
        // Non utilizzato in questa implementazione automatica
        // Il lancio avviene tramite Update() automaticamente
    }

    // ===== DEBUG VISUALIZATION =====
    
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
        // Disegna range di detection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, enemyDetectionRange);
        
        // Disegna range di lancio
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxThrowRange);
    }
}