using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Gestisce lo stato globale del gioco, le ondate, XP, eventi di gioco, ecc.
public class GameManager : MonoBehaviour
{
    // Eventi pubblici per comunicare con altri sistemi
    public UnityEvent OnEnemyKilled;
    public UnityEvent OnGameTimeChanged;
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.Playing;
    private GameTimeDisplay gameTimeDisplay;

    [SerializeField] private float gameTime = 0f; // Tempo di gioco trascorso
    [SerializeField] private int currentWave = 0; // Numero wave corrente
    [SerializeField] private int enemiesKilled = 0; // Nemici uccisi
    [SerializeField] private float XP = 0; // XP accumulata

    // Wave System Integration
    [Header("Wave System")]
    [SerializeField] private bool useTimeBasedWaves = true; // Se usare ondate a tempo
    [SerializeField] private float waveInterval = 60f; // Durata di ogni wave (se time-based)
    [SerializeField] private float timeToSaveTheCat = 10f; // Tempo per salvare il gatto (feature custom)
    private int spawnerManagedWave = 0; // Wave gestita dallo spawner

    // Attributi per Drop e Medikit
    private float attractRadius = 1.5f;
    private float attractSpeed = 2f;
    private float healAmount = 0.3f; // Percentuale di vita ripristinata

    private int PlayerLevel = 1;
    private float xpNeededToLevelUp = 30f;

    private float previousGameTime = 0f;
    private string formattedTime = "00:00";
    private bool hasSavedTheCat = true;

    // Eventi aggiuntivi
    public UnityEvent<int> OnWaveChanged;
    public UnityEvent<float> OnXPChanged;
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnPlayerLevelUp;
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent<int> OnWaveCompleted;
    public UnityEvent OnAllWavesCompleted;
    public UnityEvent OnBossWaveStarted;
    public UnityEvent<GameObject> OnBossSpawned;
    public UnityEvent OnBossDefeated;
    public UnityEvent<string> OnBossIntro;

    private bool needsReinitialization = false;

    // Controlla se il gatto è stato salvato (feature custom)
    private bool isCatSaved()
    {
        float time = GetGameTime();
        if (time > 60f * timeToSaveTheCat)
        {
            hasSavedTheCat = false;
        }
        return hasSavedTheCat;
    }

    // Singleton pattern e inizializzazione
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                needsReinitialization = true;
            }
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeGameManager();
    }

    // Inizializza eventi e componenti
    private void InitializeGameManager()
    {
        gameTimeDisplay = gameObject.GetComponent<GameTimeDisplay>();
        if (gameTimeDisplay == null)
        {
            gameTimeDisplay = gameObject.AddComponent<GameTimeDisplay>();
        }
        OnEnemyKilled = new UnityEvent();
        OnGameTimeChanged = new UnityEvent();
        OnWaveChanged = new UnityEvent<int>();
        OnXPChanged = new UnityEvent<float>();
        OnGameOver = new UnityEvent();
        OnPlayerLevelUp = new UnityEvent<int>();
        OnWaveStarted = new UnityEvent<int>();
        OnWaveCompleted = new UnityEvent<int>();
        OnAllWavesCompleted = new UnityEvent();
        OnBossWaveStarted = new UnityEvent();
        OnBossSpawned = new UnityEvent<GameObject>();
        OnBossDefeated = new UnityEvent();
        OnBossIntro = new UnityEvent<string>();
    }

    // Reset stato quando si carica la scena di gioco
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            ResetGameState();
            Invoke(nameof(ReinitializeConnections), 0.1f);
        }
    }

    // Reset di tutte le variabili di gioco
    private void ResetGameState()
    {
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
        attractRadius = 1.5f;
        attractSpeed = 2f;
        healAmount = 0.3f;
    }

    // Ricollega eventi e riferimenti dopo il reset
    private void ReinitializeConnections()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDead.RemoveListener(GameOver);
            Player.Instance.OnPlayerDead.AddListener(GameOver);
        }
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && !useTimeBasedWaves)
        {
            spawner.OnWaveStarted += OnSpawnerWaveStarted;
            spawner.OnWaveCompleted += OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted += OnSpawnerAllWavesCompleted;
            spawner.OnBossWaveStarted += () => OnBossWaveStarted?.Invoke();
            spawner.OnBossSpawned += (boss) => OnBossSpawned?.Invoke(boss);
            spawner.OnBossDefeated += () => OnBossDefeated?.Invoke();
            spawner.OnBossIntro += (intro) => OnBossIntro?.Invoke(intro);
        }
    }

    // Inizializza eventi all'avvio
    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDead.AddListener(GameOver);
        }
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && !useTimeBasedWaves)
        {
            spawner.OnWaveStarted += OnSpawnerWaveStarted;
            spawner.OnWaveCompleted += OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted += OnSpawnerAllWavesCompleted;
            spawner.OnBossWaveStarted += () => OnBossWaveStarted?.Invoke();
            spawner.OnBossSpawned += (boss) => OnBossSpawned?.Invoke(boss);
            spawner.OnBossDefeated += () => OnBossDefeated?.Invoke();
            spawner.OnBossIntro += (intro) => OnBossIntro?.Invoke(intro);
        }
    }

    // Aggiornamento ad ogni frame
    private void Update()
    {
        if (CurrentGameState != GameState.Playing) return;
        gameTime += Time.deltaTime;
        if (Mathf.FloorToInt(gameTime) != Mathf.FloorToInt(previousGameTime))
        {
            formattedTime = GameTimeDisplay.FormatGameTime(gameTime);
            OnGameTimeChanged?.Invoke();
        }
        if (useTimeBasedWaves)
        {
            CheckWaveProgression();
        }
    }

    // Gestisce la progressione delle wave a tempo
    private void CheckWaveProgression()
    {
        int newWave = Mathf.FloorToInt(gameTime / waveInterval) + 1;
        if (newWave != currentWave)
        {
            currentWave = newWave;
            OnWaveChanged?.Invoke(currentWave);
            OnWaveStarted?.Invoke(currentWave);
        }
    }

    // Eventi dallo spawner
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
    }

    // Gestione XP e level up
    public void AddXP(float xp)
    {
        XP += xp;
        if (XP >= xpNeededToLevelUp)
        {
            AudioManager.Instance?.PlayLevelUpSound();
            PlayerLevel++;
            OnPlayerLevelUp?.Invoke(PlayerLevel);
            XP -= xpNeededToLevelUp;
            xpNeededToLevelUp *= 1.15f;
        }
        OnXPChanged?.Invoke(XP);
    }

    // Incrementa il conteggio dei nemici uccisi
    public void EnemyKilled()
    {
        enemiesKilled++;
        OnEnemyKilled?.Invoke();
    }

    // Getter vari
    public int getEnemiesKilled() => enemiesKilled;
    public void GameOver()
    {
        OnGameOver?.Invoke();
        CurrentGameState = GameState.GameOver;
    }
    public float GetGameTime() => gameTime;
    public string getFormattedGameTime() => formattedTime;
    public float GetXPToLevelUp() => xpNeededToLevelUp;
    public float GetXP() => XP;
    public float GetXPNeededToLevelUp() => xpNeededToLevelUp - XP;
    public float GetAttractSpeed() => attractSpeed;
    public float GetAttractRadius() => attractRadius;
    public void SetAttractSpeed(float speed) => attractSpeed = speed;
    public void SetAttractRadius(float radius) => attractRadius = radius;
    public void SetHealAmount(float amount) => healAmount = amount;
    public float GetHealAmount() => healAmount;

    // Metodi per il sistema wave
    public int GetCurrentWave() => currentWave;
    public bool IsUsingTimeBasedWaves() => useTimeBasedWaves;
    public void SetWaveSystem(bool timeBasedWaves, float newWaveInterval = 60f)
    {
        useTimeBasedWaves = timeBasedWaves;
        waveInterval = newWaveInterval;
    }
    public void ForceWave(int waveNumber)
    {
        if (waveNumber > 0)
        {
            currentWave = waveNumber;
            OnWaveChanged?.Invoke(currentWave);
            OnWaveStarted?.Invoke(currentWave);
        }
    }
    public int GetPlayerLevel() => PlayerLevel;
    public float GetCurrentXP() => XP;

    // Gestione pausa
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
    public bool IsGamePaused() => CurrentGameState == GameState.Paused;

    // Cleanup eventi
    private void OnDestroy()
    {
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            spawner.OnWaveStarted -= OnSpawnerWaveStarted;
            spawner.OnWaveCompleted -= OnSpawnerWaveCompleted;
            spawner.OnAllWavesCompleted -= OnSpawnerAllWavesCompleted;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}