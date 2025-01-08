using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
    float Health { get; }
    float MaxHealth { get; }
    bool isAlive { get; }
    void Die();
}