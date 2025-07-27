using UnityEngine;

[CreateAssetMenu(fileName = "SniperProjectileSO", menuName = "Scriptable Objects/Projectiles/Sniper Projectile")]
public class SniperProjectileSO : ProjectileSO
{
    [Header("Sniper Bullet Properties")]
    public float pierceCount = 0f;      
    public bool showTrail = true;       
    public float trailDuration = 0.2f;  
}
