using UnityEngine;

[CreateAssetMenu(fileName = "ThrowingWeapon", menuName = "ScriptableObjects/ThrowingWeapon", order = 4)]
public class ThrowingWeaponSO : ScriptableObject
{
    [Header("Basic Stats")]
    public float damage = 25f;
    public float throwSpeed = 8f;
    public float throwCooldown = 2f;
    
    [Header("Range & Penetration")]
    public float maxThrowRange = 4f;
    public int maxPenetrations = 3;
    public float enemyDetectionRange = 8f;
    
    [Header("Visual")]
    public float rotationSpeed = 540f;
    public float returnDelay = 0.2f;
    
    [Header("Projectile")]
    public GameObject throwingProjectilePrefab;
}