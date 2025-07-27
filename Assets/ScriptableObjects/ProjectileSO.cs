using UnityEngine;
[ CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile", order = 1)]
public class ProjectileSO : ScriptableObject
{
    public float speed;
    public float damage;
    public float lifeTime;
    public GameObject projectilePrefab;
}
