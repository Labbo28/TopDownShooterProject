using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;

    [SerializeField] private Transform weaponPrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Transform pivotPoint;

    public UnityEvent<int, int> OnAmmoChanged;

    private int currentAmmo;
    private float fireRate;
    private float reloadTime;
    private int maxAmmo;
    private float lastShotTime = 0f;
    private InputSystem_Actions inputActions;

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

    public float FireRate
    {
        get => fireRate;
        set
        {
            fireRate = value;
            if (fireRateTimer != null)
                fireRateTimer = new CountdownTimer(fireRate);
        }
    }

    public float ReloadTime
    {
        get => reloadTime;
        set
        {
            reloadTime = value;
            if (reloadTimer != null)
                reloadTimer = new CountdownTimer(reloadTime);
        }
    }

    public int MaxAmmo
    {
        get => maxAmmo;
        set => maxAmmo = value;
    }

    public float LastShotTime
    {
        get => lastShotTime;
        set => lastShotTime = value;
    }

    private CountdownTimer reloadTimer;
    private CountdownTimer fireRateTimer;
    private bool isReloading = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (this is SpinWeapon || this is AreaWeapon)
        {
            return;
        }

        fireRate = weaponSo.fireRate;
        reloadTime = weaponSo.reloadTime;
        maxAmmo = weaponSo.maxAmmo;

        reloadTimer = new CountdownTimer(reloadTime);
        fireRateTimer = new CountdownTimer(fireRate);
    }

    private void Start()
    {
        if (!(this is SpinWeapon) && !(this is AreaWeapon))
        {
            GameObject shotPointObject = GameObject.Find("shotPoint");
            if (shotPointObject != null)
            {
                shotPointObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                Debug.LogWarning("shotPoint non trovato nella scena!");
            }
            currentAmmo = maxAmmo;
            UpdateTimersWithModifiers();
        }
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    public void UpdateTimersWithModifiers()
    {
        if (Player.Instance != null && !(this is SpinWeapon))
        {
            RangedWeaponStatsModifier modifier = Player.Instance.GetComponent<RangedWeaponStatsModifier>();
            if (modifier != null)
            {
                FireRate = weaponSo.fireRate * modifier.FireRateMultiplier;
                ReloadTime = weaponSo.reloadTime * modifier.ReloadSpeedMultiplier;
            }
            else if (weaponSo == null)
            {
                Debug.LogError($"WeaponSO non assegnato su {gameObject.name}!");
            }
        }
    }

    void Update()
    {
        if (Player.Instance == null || !Player.Instance.GetComponent<HealthSystem>().IsAlive)
        {
            return;
        }
        HandleManualReload();
        HandleShooting();
        HandleWeaponRotation();
        UpdateReloading();
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        reloadTimer?.Tick(Time.deltaTime);
        fireRateTimer?.Tick(Time.deltaTime);
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
            weaponSprite.flipY = mouseWorldPosition.x < pivotPoint.position.x;
        }
    }

    private void HandleManualReload()
    {
        if (inputActions.Player.Reload.WasPressedThisFrame())
        {
            Reload();
        }
    }

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
            currentAmmo = maxAmmo;
            isReloading = false;
        }
    }

    public void HandleShooting()
    {
        if (inputActions.Player.Attack.IsPressed() && fireRateTimer.IsFinished)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
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
