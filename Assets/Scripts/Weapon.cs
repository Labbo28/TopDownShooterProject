
using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;

    [SerializeField] Transform weaponPrefab;
    [SerializeField] Transform shotPoint;

    [SerializeField] private Transform pivotPoint;
    
    void Update()
    {
        HandleShooting();
        HandleWeaponRotation();
    }

    
        public Vector3 GetProjectileDirection()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;

            Vector3 direction = (mouseWorldPosition - shotPoint.position).normalized;
            return direction;
        }

   private void HandleWeaponRotation()
{
    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPosition.z = 0f; 
    
    Vector3 direction = mouseWorldPosition - pivotPoint.position;
    
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
    weaponPrefab.rotation = Quaternion.Euler(0f, 0f, angle );
    
    
    float weaponDistance = 0.6f; 
    Vector3 weaponPosition = pivotPoint.position + (direction.normalized * weaponDistance);
    weaponPrefab.position = weaponPosition;

    SpriteRenderer weaponSprite = weaponPrefab.GetComponent<SpriteRenderer>();
    if (weaponSprite != null)
    {
        if (mouseWorldPosition.x < pivotPoint.position.x)
        {
          
            weaponSprite.flipY = true;
        }
        else
        {
            weaponSprite.flipY = false;
        }
    }
}

    protected abstract void HandleShooting();
    
    public void SetWeaponSO(WeaponSO weaponSo)
    {
        this.weaponSo = weaponSo;
    }

    public WeaponSO GetWeaponSO()
    {
        return weaponSo;
    }
    public Transform GetPivotPoint()
    {
        return pivotPoint;
    }

    public Transform GetShotPoint()
    {
        return shotPoint;
    }

    public Transform GetWeaponPrefab()
    {
        return weaponPrefab;
    }
   

    
}
