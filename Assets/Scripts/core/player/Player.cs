using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float rotationSpeed = 3f;
 
    private HealthSystem healthSystem;
    private CountdownTimer dashTimer;
    private Vector2 movementDirection;
    private Vector3 initialPosition = Vector3.zero;

    private void Awake()
    {
        // Se esiste già un'istanza e non è questa
        if (Instance != null && Instance != this)
        {
            // Se stiamo caricando la GameScene, distruggi l'istanza vecchia e usa questa nuova
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                Debug.Log("Destroying old Player instance and using new one from scene");
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Altrimenti distruggi questa nuova istanza (comportamento normale del Singleton)
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            // Prima istanza o istanza nulla
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        dashTimer = new CountdownTimer(dashCooldown);
        
        // Get or add HealthSystem component
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }
        
        // Setup death event
        healthSystem.onDeath.AddListener(OnPlayerDeath);
        
        // Salva la posizione iniziale
        initialPosition = transform.position;
        
        Debug.Log($"Player initialized at position: {initialPosition}");
    }

    private void Start()
    {
        if (dashTimer != null)
        {
            dashTimer.OnTimerStop += ResetDashTimer;
        }
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
        Debug.Log("Player has died.");
        OnPlayerDead?.Invoke();
        DisablePlayerControls();
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
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
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

    // Metodo pubblico per resettare il player (opzionale, se preferisci il reset invece della distruzione)
    public void ResetPlayer()
    {
        Debug.Log("Resetting Player...");
        
        // Reset salute se esiste un metodo nel HealthSystem
        if (healthSystem != null)
        {
            // Qui dovresti aggiungere un metodo ResetHealth() nel HealthSystem
            // healthSystem.ResetHealth();
        }
        
        // Reset posizione
        transform.position = initialPosition;
        
        // Reset timer dash
        if (dashTimer != null && dashTimer.IsRunning)
        {
            dashTimer.Stop();
            dashTimer.Reset();
        }
        
        // Riattiva controlli
        EnablePlayerControls();
        
        // Reset eventi di animazione
        OnPlayerStopMoving?.Invoke();
        
        Debug.Log($"Player reset completed. Position: {transform.position}");
    }

    // Metodo statico per distruggere l'istanza corrente (utile per il reset completo)
    public static void DestroyCurrentInstance()
    {
        if (Instance != null)
        {
            Debug.Log("Destroying current Player instance");
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    private void OnDestroy()
    {
        // Cleanup del timer
        if (dashTimer != null)
        {
            dashTimer.OnTimerStop -= ResetDashTimer;
        }
        
        // Se questa istanza viene distrutta, resetta il singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}