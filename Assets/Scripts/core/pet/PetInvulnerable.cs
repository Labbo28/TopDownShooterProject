using UnityEngine;

/// <summary>
/// Makes the attached entity invulnerable to any damage attempts.
/// If a HealthSystem is present, incoming damage is ignored.
/// Also ignores collisions from enemy projectiles.
/// </summary>
public class PetInvulnerable : MonoBehaviour, IDamageable
{
    // IDamageable contract
    public float Health => float.PositiveInfinity;
    public float MaxHealth => float.PositiveInfinity;
    public bool IsAlive => true;

    public void TakeDamage(float damage)
    {
        // Intentionally do nothing: pet is invulnerable
    }

    public void Die()
    {
        // Pet never dies; ignore
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If hit by enemy projectiles, ignore and optionally destroy the projectile so it doesn't linger
        if (other.CompareTag("EnemyProjectile"))
        {
            // Ignore collision to prevent physics interactions
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                Physics2D.IgnoreCollision(other, myCollider);
            }

            // Optionally destroy the projectile so it doesn't keep flying through
            // Comment this out if you prefer projectiles to pass through without being destroyed
            if (other.TryGetComponent<Projectile>(out var projectile))
            {
                Destroy(projectile.gameObject);
            }
        }
    }
}


