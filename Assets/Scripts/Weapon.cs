
using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;

    [SerializeField] Transform weaponPrefab;
    [SerializeField] Transform shotPoint;
    private float offset = 90f;

     void Update()
    {
        HandleShooting();
        HandleWeaponRotation();
    }

    private void HandleWeaponRotation()
    {
        Vector3 displacement = weaponPrefab.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        weaponPrefab.rotation = Quaternion.Euler(0f, 0f, angle+offset);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) )
        {
            Instantiate( weaponSo.projectile.projectilePrefab, shotPoint.position, shotPoint.rotation);
        }
    }
}
