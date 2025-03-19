using UnityEngine;

[CreateAssetMenu(fileName = "PoisonProjectileSO", menuName = "Scriptable Objects/Projectiles/Poison Projectile")]
public class PoisonProjectileSO : ProjectileSO
{
    [Header("Poison Properties")]
    public float poisonDuration = 5f;
    public float poisonTickRate = 1f;
    public float poisonDamagePerTick = 5f;
    public Color poisonColor = new Color(0.5f, 1f, 0f, 1f);
}