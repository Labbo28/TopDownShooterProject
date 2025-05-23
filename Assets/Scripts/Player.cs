using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashCooldown = 3f;
    [SerializeField] private float rotationSpeed = 3f;
 
    private HealthSystem healthSystem;
    private CountdownTimer _dashTimer;
  
    private Vector2 _movementDirection;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _dashTimer = new CountdownTimer(_dashCooldown);
        
        // Get or add HealthSystem component
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }
        
        // Setup death event
        healthSystem.onDeath.AddListener(OnPlayerDeath);
    }

    private void Start()
    {
        if (_dashTimer != null)
        {
            _dashTimer.OnTimerStop += ResetDashTimer;
        }
    }

    private void Update()
    {
        // Skip updates if player is dead
        if (healthSystem != null && !healthSystem.IsAlive) return;
               
        // Tick the timer and handle movement
        _dashTimer?.Tick(Time.deltaTime);  
        
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
    }

    private void HandleMovement()
    {
        _movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (_movementDirection.magnitude > 0)
        {
            OnPlayerMoving?.Invoke();
        }
        else
        {
            OnPlayerStopMoving?.Invoke();
        }

        transform.Translate(_movementDirection * (Time.deltaTime * _movementSpeed));
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dashDirection = new Vector3(_movementDirection.x, _movementDirection.y).normalized;
            Vector3 dashTarget = transform.position + dashDirection * DashDistance;

            StartCoroutine(DashMovement(dashTarget, DashDuration));
            _dashTimer?.Start();
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
        return _dashTimer != null && !_dashTimer.IsRunning;
    }

    private void ResetDashTimer()
    {
        _dashTimer?.Reset();
    }
}