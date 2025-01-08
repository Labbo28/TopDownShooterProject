using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    float Health { get; }
    float MaxHealth { get; }
    bool isAlive { get; }
    void Die();
}