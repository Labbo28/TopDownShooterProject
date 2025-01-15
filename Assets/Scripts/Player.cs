using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    // Singleton
    public static Player Instance { get; private set; }

    // Costanti
    private const float DashDistance = 3f;
    private const float DashDuration = 0.2f;

    // Campi private
    [SerializeField] private float _currentHealth = 100f;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashCooldown = 3f;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private float rotationSpeed = 3f;

    

    private CountdownTimer _dashTimer;
    private float _nextShotTime;
    private Vector2 _movementDirection;

    public float Health => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool isAlive { get; }
    public bool IsAlive => _currentHealth > 0;

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
        // Tick del timer e movimento
        _dashTimer?.Tick(Time.deltaTime);
        HandleMovement();

        // Controllo Dash
        if (CanDash())
        {
            HandleDash();
        }

       
    }
    

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }

    private void HandleMovement()
    {
        _movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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