
using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;

    [SerializeField] Transform weaponPrefab;
    [SerializeField] Transform shotPoint;

    [SerializeField] private Transform pivotPoint;

    private int currentAmmo;
    private float lastShotTime = 0f;

    public WeaponSO WeaponSo
    {
        get => weaponSo;
        set => weaponSo = value;
    }

    public Transform WeaponPrefab
    {
        get => weaponPrefab;
        set => weaponPrefab = value;
    }

    public Transform ShotPoint
    {
        get => shotPoint;
        set => shotPoint = value;
    }

    public Transform PivotPoint
    {
        get => pivotPoint;
        set => pivotPoint = value;
    }

    public int CurrentAmmo
    {
        get => currentAmmo;
        set => currentAmmo = value;
    }

    public float LastShotTime
    {
        get => lastShotTime;
        set => lastShotTime = value;
    }

    private CountdownTimer reloadTimer;
    private CountdownTimer fireRateTimer;

    private void Start()
    {
        currentAmmo = weaponSo.maxAmmo;
    }
    private void Awake()
    {
    
        reloadTimer = new CountdownTimer(weaponSo.reloadTime);
        fireRateTimer = new CountdownTimer(weaponSo.fireRate);
    }

    void Update()
    {
        Debug.Log("Fire Rate Timer: " + fireRateTimer.GetTime());
        Debug.Log("Current Ammo: " + currentAmmo);
        HandleManualReload();
        HandleShooting();
        HandleWeaponRotation();
         reloadTimer.Tick(Time.deltaTime);
        fireRateTimer.Tick(Time.deltaTime);
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

        weaponPrefab.rotation = Quaternion.Euler(0f, 0f, angle);

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
    
    private void HandleManualReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
      private void Reload()
    {
        if (reloadTimer.IsFinished)
        {
            currentAmmo = weaponSo.maxAmmo;
            reloadTimer.Start();
        }
    }

    public void HandleShooting()
    {
        if (Input.GetMouseButton(0) && fireRateTimer.IsFinished)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                fireRateTimer.Start();
            }
            else
            {
                Reload();
            }
        }
    }

    protected abstract void Shoot();
}
