using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Eventi per la gestione delle animazioni
    public UnityEvent OnPlayerMoving;
    public UnityEvent OnPlayerStopMoving;
    public UnityEvent OnPlayerDead;

    // Singleton
    public static Player Instance { get; private set; }

    // Constants
    private const float DashDistance = 3f;
    private const float DashDuration = 0.2f;

    // Fields
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float rotationSpeed = 3f;

    private HealthSystem healthSystem;
    private CountdownTimer dashTimer;
    private Vector2 movementDirection;
    private Vector3 initialPosition = Vector3.zero;
    private bool isDead = false;
    private InputSystem_Actions inputActions;
    // Animator gestito da PlayerAnimator component

    private void Awake()
    {
        // Gestione del Singleton più semplice
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Iscriviti agli eventi di caricamento scene
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializePlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            ResetPlayer();
        }
        if (scene.name == "GameScene_second" || "PlainToForest" == scene.name || scene.name == "ForestToPlain")
        {
            FindSpawnPosition();
        }
    }

    private void InitializePlayer()
    {
        dashTimer = new CountdownTimer(dashCooldown);
        inputActions = new InputSystem_Actions();

        // Animator gestito da PlayerAnimator component

        // Get or add HealthSystem component
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }

        // Setup death event
        healthSystem.onDeath.RemoveListener(OnPlayerDeath); // Rimuovi prima per evitare duplicati
        healthSystem.onDeath.AddListener(OnPlayerDeath);

        // Trova la posizione di spawn corretta
        FindSpawnPosition();
        isDead = false;

    }

    private void Start()
    {
        if (dashTimer != null)
        {
            dashTimer.OnTimerStop += ResetDashTimer;
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

    private void Update()
    {
        // Skip updates if player is dead
        if (healthSystem != null && !healthSystem.IsAlive) return;

        // Tick the timer and handle movement
        dashTimer?.Tick(Time.deltaTime);

        HandleMovement();

        // Check for dash
        if (CanDash())
        {
            HandleDash();
        }
    }

    private void OnPlayerDeath()
    {
        if (isDead) return; // Evita chiamate multiple

        isDead = true;

        // Animazione di morte gestita da PlayerAnimator tramite eventi
        DisablePlayerControls();
        OnPlayerDead?.Invoke();
        transform.Find("Weapons")?.gameObject.SetActive(false);
        
    }

    private void DisablePlayerControls()
    {
        // Disabilita il sistema delle armi
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.enabled = false;
        }

        // Disabilita LookAtCursor se presente
        LookAtCursor lookAtCursor = GetComponentInChildren<LookAtCursor>();
        if (lookAtCursor != null)
        {
            lookAtCursor.enabled = false;
        }
    }

    private void EnablePlayerControls()
    {
        // Riattiva il sistema delle armi
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.enabled = true;
        }

        // Riattiva LookAtCursor se presente
        LookAtCursor lookAtCursor = GetComponentInChildren<LookAtCursor>();
        if (lookAtCursor != null)
        {
            lookAtCursor.enabled = true;
        }
    }

    private void HandleMovement()
    {
        movementDirection = inputActions.Player.Move.ReadValue<Vector2>();
        if (movementDirection.magnitude > 0)
        {
            OnPlayerMoving?.Invoke();
        }
        else
        {
            OnPlayerStopMoving?.Invoke();
        }

        transform.Translate(movementDirection * (Time.deltaTime * movementSpeed));
    }

    private void HandleDash()
    {
        if (inputActions.Player.Sprint.WasPressedThisFrame())
        {
            Vector3 dashDirection = new Vector3(movementDirection.x, movementDirection.y).normalized;
            Vector3 dashTarget = transform.position + dashDirection * DashDistance;

            StartCoroutine(DashMovement(dashTarget, DashDuration));
            dashTimer?.Start();
        }
    }

    private IEnumerator DashMovement(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private bool CanDash()
    {
        return dashTimer != null && !dashTimer.IsRunning;
    }

    private void ResetDashTimer()
    {
        dashTimer?.Reset();
    }

    // Metodo per trovare la posizione di spawn nella scena corrente
    private void FindSpawnPosition()
    {
        // Cerca un oggetto con tag "PlayerSpawn"
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnPoint != null)
        {
            initialPosition = spawnPoint.transform.position;
            transform.position = initialPosition;
        }
        else
        {
            // Se non c'è uno spawn point specifico, usa una posizione di default o quella corrente
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player != gameObject)
            {
                // Se c'è un altro player nella scena (dalla scena), usa la sua posizione
                initialPosition = player.transform.position;
                transform.position = initialPosition;
            }
            else
            {
                // Altrimenti usa (0,0,0) o la posizione corrente se ragionevole
                if (transform.position == Vector3.zero || Vector3.Distance(transform.position, Vector3.zero) > 100f)
                {
                    initialPosition = Vector3.zero;
                    transform.position = initialPosition;
                }
                else
                {
                    initialPosition = transform.position;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            healthSystem?.TakeDamage(other.GetComponent<Projectile>().Damage);
            Destroy(other.gameObject);
        }
    }
    // Metodo per resettare completamente il player
    public void ResetPlayer()
    {

        isDead = false;
        transform.Find("Weapons")?.gameObject.SetActive(true);

        // Reset HealthSystem usando il nuovo metodo
        if (healthSystem != null)
        {
            healthSystem.ResetHealth();
            // Assicurati che l'evento di morte sia collegato
            healthSystem.onDeath.RemoveListener(OnPlayerDeath);
            healthSystem.onDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            // Se per qualche motivo l'HealthSystem è null, crealo
            InitializePlayer();
        }

        // Reset dell'Animator gestito da PlayerAnimator component
        PlayerAnimator playerAnimatorComponent = GetComponent<PlayerAnimator>();
        if (playerAnimatorComponent != null)
        {
            playerAnimatorComponent.ResetAnimator();
        }

        // Trova la nuova posizione di spawn nella scena corrente
        FindSpawnPosition();

        // Reset timer dash
        if (dashTimer != null)
        {
            if (dashTimer.IsRunning)
            {
                dashTimer.Stop();
            }
            dashTimer.Reset();
        }

        // Riattiva controlli
        EnablePlayerControls();

        // Reset eventi di animazione
        OnPlayerStopMoving?.Invoke();

        // Reset dei componenti di upgrade personalizzati
        ResetUpgradeComponents();

    }

    private void ResetUpgradeComponents()
    {
        // Reset RangedWeaponStatsModifier
        RangedWeaponStatsModifier rangedWeaponStats = GetComponent<RangedWeaponStatsModifier>();
        if (rangedWeaponStats != null)
        {
            rangedWeaponStats.ResetModifiers();
        }

        // Reset HealingBoostComponent
        HealingBoostComponent healingBoost = GetComponent<HealingBoostComponent>();
        if (healingBoost != null)
        {
            healingBoost.ResetMultiplier();
        }

        // Reset SpinWeaponStatsModifier
        SpinWeaponStatsModifier spinWeaponStats = GetComponent<SpinWeaponStatsModifier>();
        if (spinWeaponStats != null)
        {
            spinWeaponStats.ResetModifiers();
        }

        // MagnetDropBooster rimosso

        // Rimuovi HealthRegenComponent se presente
        HealthRegenComponent healthRegen = GetComponent<HealthRegenComponent>();
        if (healthRegen != null)
        {
            Destroy(healthRegen);
        }

        // MagnetVisualEffect rimosso dal sistema

        // Reset velocità di movimento al valore base
        SetMovementSpeed(4f); // Velocità base originale
        
        // Reset dash cooldown al valore base
        SetDashCooldown(3f); // Cooldown base originale
        
        // Reset weapon unlocks
        WeaponUnlockManager weaponUnlockManager = GetComponent<WeaponUnlockManager>();
        if (weaponUnlockManager != null)
        {
            weaponUnlockManager.ResetUnlockedWeapons();
        }

    }

    private void OnDestroy()
    {
        // Cleanup del timer
        if (dashTimer != null)
        {
            dashTimer.OnTimerStop -= ResetDashTimer;
        }

        // Rimuovi l'iscrizione agli eventi di scena
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Se questa istanza viene distrutta, resetta il singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }

    public CountdownTimer GetDashTimer() => dashTimer;

    public float GetDashCooldown() => dashCooldown;

    public void SetDashCooldown(float newCooldown)
    {
        dashCooldown = newCooldown;
        if (dashTimer != null)
        {
            dashTimer = new CountdownTimer(dashCooldown);
        }
    }
    

}