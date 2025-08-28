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
    private bool isDead = false;
    private Animator playerAnimator;

    private void Awake()
    {
        // Gestione del Singleton più semplice
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying duplicate Player instance");
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
            Debug.Log("GameScene loaded, resetting Player");
            ResetPlayer();
        }
    }

    private void InitializePlayer()
    {
        dashTimer = new CountdownTimer(dashCooldown);
        
        // Get the Animator component
        playerAnimator = GetComponent<Animator>();
        
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
        if (isDead) return; // Evita chiamate multiple
        
        isDead = true;
        Debug.Log("Player has died.");
        
        // Imposta l'animazione di morte
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isDead", true);
        }
        
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

    // Metodo per trovare la posizione di spawn nella scena corrente
    private void FindSpawnPosition()
    {
        // Cerca un oggetto con tag "PlayerSpawn"
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnPoint != null)
        {
            initialPosition = spawnPoint.transform.position;
            transform.position = initialPosition;
            Debug.Log($"Found PlayerSpawn at: {initialPosition}");
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
                Debug.Log($"Using existing Player position: {initialPosition}");
            }
            else
            {
                // Altrimenti usa (0,0,0) o la posizione corrente se ragionevole
                if (transform.position == Vector3.zero || Vector3.Distance(transform.position, Vector3.zero) > 100f)
                {
                    initialPosition = Vector3.zero;
                    transform.position = initialPosition;
                    Debug.Log("Using default spawn position: (0,0,0)");
                }
                else
                {
                    initialPosition = transform.position;
                    Debug.Log($"Using current position as spawn: {initialPosition}");
                }
            }
        }
    }

    // Metodo per resettare completamente il player
    public void ResetPlayer()
    {
        Debug.Log("Resetting Player...");
        
        isDead = false;
        
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
        
        // Reset dell'Animator
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isDead", false);
            playerAnimator.SetBool("isRunning", false);
            // Reset eventuali trigger
            playerAnimator.ResetTrigger("Hit");
            Debug.Log("Animator reset completed");
        }
        else
        {
            // Prova a trovare l'animator se è null
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isDead", false);
                playerAnimator.SetBool("isRunning", false);
                playerAnimator.ResetTrigger("Hit");
            }
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
        
        Debug.Log($"Player reset completed. Position: {transform.position}, Health: {healthSystem?.Health}/{healthSystem?.MaxHealth}");
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
}