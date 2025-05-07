using UnityEngine;

public class HandCannon : Weapon
{

    private float spreadAngle = 0f;
    protected override void Shoot()
    {
        float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject projectile = Instantiate(
            WeaponSo.projectile.projectilePrefab,
            ShotPoint.position,
            ShotPoint.rotation * rotation
        );
    }

}
