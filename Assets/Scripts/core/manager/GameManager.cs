using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnEnemyKilled;
    public UnityEvent OnGameTimeChanged;

    
    public static GameManager Instance { get; private set; }

    private GameTimeDisplay gameTimeDisplay;

    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private float XP = 0;

    // Wave System Integration
    [Header("Wave System")]
    [SerializeField] private bool useTimeBasedWaves = true;
    [SerializeField] private float waveInterval = 60f; // secondi per wave se time-based
    private int spawnerManagedWave = 0; // wave gestita dallo spawner
    
    //attrtributi della classe MagnetDrop
    private float attractRadius = 1.5f;
    private float attractSpeed = 2f;

    //Attributi della classe Medikit_drop
    private float healAmount = 0.3f; //valore percentuale della vita ripristinata

    private int PlayerLevel = 1;
    private float xpNeededToLevelUp = 10;
    
    private float previousGameTime = 0f;
    private string formattedTime = "00:00";

    public UnityEvent<int> OnWaveChanged;
    public UnityEvent<float> OnXPChanged;
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnPlayerLevelUp;
    
    // New events for wave system
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent<int> OnWaveCompleted;
    public UnityEvent OnAllWavesCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameTimeDisplay = gameObject.AddComponent<GameTimeDisplay>();

        // Initialize events
        OnEnemyKilled = new UnityEvent();
        OnGameTimeChanged = new UnityEvent();
        OnWaveChanged = new UnityEvent<int>();
        OnXPChanged = new UnityEvent<float>();
        OnGameOver = new UnityEvent();
        OnPlayerLevelUp = new UnityEvent<int>();
        OnWaveStarted = new UnityEvent<int>();
        OnWaveCompleted = new UnityEvent<int>();
        OnAllWavesCompleted = new UnityEvent();
    }
    

    private void Start()
    {
        // Registra agli eventi dello spawner se presente
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && !useTimeBasedWaves)
        {
            spawner.OnWaveStarted += OnSpawnerWaveStarted;
            spawner.OnWaveCompleted += OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted += OnSpawnerAllWavesCompleted;
        }
        Player.Instance.OnPlayerDead.AddListener(GameOver);

    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (Mathf.FloorToInt(gameTime) != Mathf.FloorToInt(previousGameTime))
        {
            formattedTime = GameTimeDisplay.FormatGameTime(gameTime);
            OnGameTimeChanged?.Invoke();
        }
        
        // Solo usa time-based waves se configurato
        if (useTimeBasedWaves)
        {
            CheckWaveProgression();
        }
    }

    private void CheckWaveProgression()
    {
        // Logica per cambiare le ondate basata sul tempo
        int newWave = Mathf.FloorToInt(gameTime / waveInterval) + 1;
        
        if (newWave != currentWave)
        {
            currentWave = newWave;
            OnWaveChanged?.Invoke(currentWave);
            OnWaveStarted?.Invoke(currentWave);
        }
    }
    
    // Metodi per gestire eventi spawner
    private void OnSpawnerWaveStarted(int waveNumber)
    {
        spawnerManagedWave = waveNumber;
        currentWave = waveNumber;
        OnWaveStarted?.Invoke(waveNumber);
        OnWaveChanged?.Invoke(waveNumber);
    }
    
    private void OnSpawnerWaveCompleted(int waveNumber)
    {
        OnWaveCompleted?.Invoke(waveNumber);
    }
    
    private void OnSpawnerAllWavesCompleted()
    {
        OnAllWavesCompleted?.Invoke();
        // Potresti voler triggerare una vittoria qui
        Debug.Log("All waves completed! Player wins!");
    }

    public void AddXP(float xp)
    {
        XP += xp;
        if (XP >= xpNeededToLevelUp)
        {
            PlayerLevel++;
            OnPlayerLevelUp?.Invoke(PlayerLevel);
            XP -= xpNeededToLevelUp;
            xpNeededToLevelUp *= 1.5f;
        }
        OnXPChanged?.Invoke(XP);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        OnEnemyKilled?.Invoke();
    }

    public int getEnemiesKilled()
    {
        return enemiesKilled;
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public float GetGameTime()
    {
        return gameTime;
    }

    public string getFormattedGameTime()
    {
        return formattedTime;
    }

    public float GetXPToLevelUp()
    {
        return xpNeededToLevelUp;
    }

    public float GetAttractSpeed()
    {
        return attractSpeed;
    }

    public float GetAttractRadius()
    {
        return attractRadius;
    }
    
    public void SetAttractSpeed(float speed)
    {
        attractSpeed = speed;
    }

    public void SetAttractRadius(float radius)
    {
        attractRadius = radius;
    }

    public void SetHealAmount(float amount)
    {
        healAmount = amount;
    }

    public float GetHealAmount()
    {
        return healAmount;
    }
    
    // Nuovi metodi per wave system
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public bool IsUsingTimeBasedWaves()
    {
        return useTimeBasedWaves;
    }
    
    public void SetWaveSystem(bool timeBasedWaves, float newWaveInterval = 60f)
    {
        useTimeBasedWaves = timeBasedWaves;
        waveInterval = newWaveInterval;
    }
    
    // Metodo per forzare una wave specifica (utile per testing)
    public void ForceWave(int waveNumber)
    {
        if (waveNumber > 0)
        {
            currentWave = waveNumber;
            OnWaveChanged?.Invoke(currentWave);
            OnWaveStarted?.Invoke(currentWave);
        }
    }
    
    private void OnDestroy()
    {
        // Cleanup degli eventi spawner
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            spawner.OnWaveStarted -= OnSpawnerWaveStarted;
            spawner.OnWaveCompleted -= OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted -= OnSpawnerAllWavesCompleted;
        }
    }
}