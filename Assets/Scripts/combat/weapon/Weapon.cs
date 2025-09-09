
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

    private void Awake()
    {
        // Initialize timers with base values - will be updated with modifiers in Start()
        reloadTimer = new CountdownTimer(weaponSo.reloadTime);
        fireRateTimer = new CountdownTimer(weaponSo.fireRate);
    }
    
    private void Start()
    {
        GameObject shotPointObject = GameObject.Find("shotPoint");
        shotPointObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        currentAmmo = weaponSo.maxAmmo;
        
        // Apply modifiers after awake to ensure Player components exist
        UpdateTimersWithModifiers();
    }
    
    public void UpdateTimersWithModifiers()
    {
        if (Player.Instance != null && !(this is SpinWeapon))
        {
            RangedWeaponStatsModifier modifier = Player.Instance.GetComponent<RangedWeaponStatsModifier>();
            if (modifier != null)
            {
                float modifiedFireRate = weaponSo.fireRate * modifier.FireRateMultiplier;
                float modifiedReloadTime = weaponSo.reloadTime * modifier.ReloadSpeedMultiplier;
                
                reloadTimer = new CountdownTimer(modifiedReloadTime);
                fireRateTimer = new CountdownTimer(modifiedFireRate);
                
                Debug.Log($"Applied weapon modifiers to {name}: FireRate={modifiedFireRate}, ReloadTime={modifiedReloadTime}");
            }
        }
    }

    void Update()
    {
         if (Player.Instance == null || !Player.Instance.GetComponent<HealthSystem>().IsAlive)
    {
        return; // Non fare nulla se il player è morto
    }
        HandleManualReload();
        HandleShooting();
        HandleWeaponRotation();
        UpdateReloading();
        UpdateTimers();
         
    }

    private void UpdateTimers()
    {
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
    private bool isReloading = false;

    private void Reload()
    {
        if (!isReloading && reloadTimer.IsFinished)
        {
            isReloading = true;
            reloadTimer.Start();
        }
    }

    private void UpdateReloading()
    {
        if (isReloading && reloadTimer.IsFinished)
        {
            currentAmmo = weaponSo.maxAmmo;
            isReloading = false;
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
