using UnityEngine;

public class Shotgun : Weapon
{
    private int numberOfPellets = 10;
    private float spreadAngle = 20f;

    protected override void Shoot()
    {
       
            for (int i = 0; i < numberOfPellets; i++)
            {
                float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                Vector3 direction = rotation * transform.up;

                // Instantiate the projectile prefab and set its direction
                GameObject projectile = Instantiate(
                    WeaponSo.projectile.projectilePrefab,
                    ShotPoint.position,
                    ShotPoint.rotation * rotation
                );

        }
    }
}
