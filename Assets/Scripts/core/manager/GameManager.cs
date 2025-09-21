using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnEnemyKilled;
    public UnityEvent OnGameTimeChanged;
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.Playing;
    private GameTimeDisplay gameTimeDisplay;

    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private float XP = 0;

    // Wave System Integration
    [Header("Wave System")]
    [SerializeField] private bool useTimeBasedWaves = true;
    [SerializeField] private float waveInterval = 60f; // secondi per wave se time-based
    [SerializeField] private float timeToSaveTheCat = 10f; 
    private int spawnerManagedWave = 0; // wave gestita dallo spawner
    
    //attrtributi della classe MagnetDrop
    private float attractRadius = 1.5f;
    private float attractSpeed = 2f;

    //Attributi della classe Medikit_drop
    private float healAmount = 0.3f; //valore percentuale della vita ripristinata

    private int PlayerLevel = 1;
    private float xpNeededToLevelUp = 30f;
    
    private float previousGameTime = 0f;
    private string formattedTime = "00:00";
    private bool hasSavedTheCat = true;
   

    public UnityEvent<int> OnWaveChanged;
    public UnityEvent<float> OnXPChanged;
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnPlayerLevelUp;
    
    // New events for wave system
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent<int> OnWaveCompleted;
    public UnityEvent OnAllWavesCompleted;

    // Boss wave events
    public UnityEvent OnBossWaveStarted;
    public UnityEvent<GameObject> OnBossSpawned;
    public UnityEvent OnBossDefeated;
    public UnityEvent<string> OnBossIntro;

    // Flag per sapere se dobbiamo reinizializzare
    private bool needsReinitialization = false;

   private bool isCatSaved()
{
    float time = GetGameTime();
    if (time > 60f * timeToSaveTheCat)
    {
        hasSavedTheCat = false; 
    }

    return hasSavedTheCat;
}


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Se stiamo ricaricando la GameScene, resettiamo lo stato
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                needsReinitialization = true;
            }
            
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Iscriviti agli eventi di caricamento scene
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeGameManager();
    }

    private void InitializeGameManager()
    {
        gameTimeDisplay = gameObject.GetComponent<GameTimeDisplay>();
        if (gameTimeDisplay == null)
        {
            gameTimeDisplay = gameObject.AddComponent<GameTimeDisplay>();
        }

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

        // Initialize boss wave events
        OnBossWaveStarted = new UnityEvent();
        OnBossSpawned = new UnityEvent<GameObject>();
        OnBossDefeated = new UnityEvent();
        OnBossIntro = new UnityEvent<string>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            ResetGameState();
            
            // Reinizializza i collegamenti dopo un frame per assicurarsi che tutto sia pronto
            Invoke(nameof(ReinitializeConnections), 0.1f);
        }
    }

    private void ResetGameState()
    {
        // Reset di tutte le variabili di gioco
        gameTime = 0f;
        currentWave = 0;
        enemiesKilled = 0;
        XP = 0;
        PlayerLevel = 1;
        xpNeededToLevelUp = 30;
        previousGameTime = 0f;
        formattedTime = "00:00";
        spawnerManagedWave = 0;
        CurrentGameState = GameState.Playing;
        Player.Instance.ResetPlayer();
        
        // Reset degli attributi drop
        attractRadius = 1.5f;
        attractSpeed = 2f;
        healAmount = 0.3f;

    }

    private void ReinitializeConnections()
    {
        // Cerca e collega il player se presente
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDead.RemoveListener(GameOver);
            Player.Instance.OnPlayerDead.AddListener(GameOver);
        }

        // Registra agli eventi dello spawner se presente
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && !useTimeBasedWaves)
        {
            spawner.OnWaveStarted += OnSpawnerWaveStarted;
            spawner.OnWaveCompleted += OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted += OnSpawnerAllWavesCompleted;

            // Connect boss wave events
            spawner.OnBossWaveStarted += () => OnBossWaveStarted?.Invoke();
            spawner.OnBossSpawned += (boss) => OnBossSpawned?.Invoke(boss);
            spawner.OnBossDefeated += () => OnBossDefeated?.Invoke();
            spawner.OnBossIntro += (intro) => OnBossIntro?.Invoke(intro);
        }
        

    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDead.AddListener(GameOver);
        }

        // Registra agli eventi dello spawner se presente
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && !useTimeBasedWaves)
        {
            spawner.OnWaveStarted += OnSpawnerWaveStarted;
            spawner.OnWaveCompleted += OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted += OnSpawnerAllWavesCompleted;

            // Connect boss wave events
            spawner.OnBossWaveStarted += () => OnBossWaveStarted?.Invoke();
            spawner.OnBossSpawned += (boss) => OnBossSpawned?.Invoke(boss);
            spawner.OnBossDefeated += () => OnBossDefeated?.Invoke();
            spawner.OnBossIntro += (intro) => OnBossIntro?.Invoke(intro);
        }
    }

    private void Update()
    {
        if (CurrentGameState != GameState.Playing) return;
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
        SceneManager.LoadScene("PlainToForest");
    }

    public void AddXP(float xp)
    {
        XP += xp;
        if (XP >= xpNeededToLevelUp)
        {
            AudioManager.Instance?.PlayLevelUpSound();
            PlayerLevel++;
            OnPlayerLevelUp?.Invoke(PlayerLevel);
            XP -= xpNeededToLevelUp;
            xpNeededToLevelUp *= 1.1f;
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
        CurrentGameState = GameState.GameOver;

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

    public float GetXP()
    {
        return XP;
    }

    public float GetXPNeededToLevelUp()
    {
        return xpNeededToLevelUp - XP;
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

    public int GetPlayerLevel()
    {
        return PlayerLevel;
    }

    public float GetCurrentXP()
    {
        return XP;
    }

    // Metodi per gestire la pausa
    public void PauseGame()
    {
        if (CurrentGameState == GameState.Playing)
        {
            CurrentGameState = GameState.Paused;
        }
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            CurrentGameState = GameState.Playing;
        }
    }

    public bool IsGamePaused()
    {
        return CurrentGameState == GameState.Paused;
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

        // Rimuovi l'iscrizione agli eventi di scena
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}