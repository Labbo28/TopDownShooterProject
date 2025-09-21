using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 1f;
    
    [Header("Spawn Limits")]
    [SerializeField] private int maxEnemiesAlive = 50;
    
    
    [SerializeField] private float minDistanceFromPlayer = 5f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Current wave state
    private int currentWaveIndex = 0;
    private Wave currentWave;
    private float waveStartTime;
    private bool waveActive = false;
    
    // Enemy tracking
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private Dictionary<SpawnData, float> lastSpawnTimes = new Dictionary<SpawnData, float>();
    private HashSet<SpawnData> spawnedBatches = new HashSet<SpawnData>();

    // Boss tracking
    private GameObject currentBoss;
    private bool bossSpawned = false;
    private bool HasInvokedBossDefeated = false;
    
    // Player reference
    private Transform playerTransform;
    
    // Events
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action OnAllWavesCompleted;
    public System.Action<GameObject> OnBossSpawned;
    public System.Action OnBossWaveStarted;
    public System.Action OnBossDefeated;
    public System.Action<string> OnBossIntro;
    
    private void FindPlayer()
    {
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Invoke(nameof(FindPlayer), 0.1f);
            }
        }
    }

    
    private void Start()
    {
        playerTransform = FindObjectOfType<Player>()?.transform;
        
        if (playerTransform == null)
        {
            return;
        }
        
        // Registra agli eventi del GameManager per sincronizzare le wave
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged.AddListener(OnGameManagerWaveChanged);
        }
        
        // Inizializza il sistema
        InitializeSpawner();
    }
    
    private void InitializeSpawner()
    {
        if (waves.Count > 0)
        {
            StartWave(0);
        }
        else
        {
        }
    }
    
    private void Update()
    {
        if (!waveActive || currentWave == null ||
         GameManager.Instance.CurrentGameState == GameState.GameOver) return;
        
        // Pulisci la lista dei nemici morti
        CleanupDeadEnemies();
        
        // Controlla se la wave è finita
        if (IsWaveComplete())
        {
            CompleteCurrentWave();
            return;
        }
        
        // Spawna nemici per la wave corrente
        ProcessCurrentWaveSpawning();
    }
    
    private void OnGameManagerWaveChanged(int newWave)
    {
        // Il GameManager potrebbe avere una numerazione diversa
        // Adatta qui la logica se necessario
        if (newWave - 1 != currentWaveIndex && newWave - 1 < waves.Count)
        {
            StartWave(newWave - 1);
        }
    }
    
    private void StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Count)
        {
            OnAllWavesCompleted?.Invoke();
            return;
        }

        currentWaveIndex = waveIndex;
        currentWave = waves[waveIndex];
        waveStartTime = Time.time;
        waveActive = true;

        // Reset spawn timers and boss state
        lastSpawnTimes.Clear();
        spawnedBatches.Clear();
        bossSpawned = false;
        currentBoss = null;
        HasInvokedBossDefeated = false;

        if (debugMode)
        {
            Debug.Log($"Starting Wave {currentWave.waveNumber}: {currentWave.waveName}");
            Debug.Log($"Wave duration: {currentWave.waveDuration}s, Spawn data count: {currentWave.spawnData.Count}");
        }

        // Special handling for boss waves
        if (currentWave.isBossWave)
        {
            OnBossWaveStarted?.Invoke();

            // Show boss intro if enabled
            if (currentWave.ShowBossIntro)
            {
                StartCoroutine(ShowBossIntroAfterDelay(currentWave.BossIntroText, 1f));
            }

            if (debugMode)
            {
            }
        }

        OnWaveStarted?.Invoke(currentWave.waveNumber);
    }
    
    private void ProcessCurrentWaveSpawning()
    {
        float waveTime = Time.time - waveStartTime;

        // Handle boss spawning for boss waves
        if (currentWave.isBossWave && !bossSpawned && waveTime >= currentWave.BossSpawnDelay)
        {
            SpawnBoss();
        }

        // Regular enemy spawning (pause if boss wave setting is enabled and boss is alive)
        bool shouldPauseRegularSpawning = currentWave.isBossWave &&
                                          currentWave.PauseRegularSpawningDuringBoss &&
                                          currentBoss != null &&
                                          currentBoss.GetComponent<HealthSystem>()?.IsAlive == true;

        if (!shouldPauseRegularSpawning)
        {
            List<SpawnData> activeSpawnData = currentWave.GetActiveSpawnData(waveTime);
            foreach (SpawnData spawnData in activeSpawnData)
            {
                ProcessSpawnData(spawnData, waveTime);
            }
        }
    }
    
    private void ProcessSpawnData(SpawnData spawnData, float waveTime)
    {
        // Controlla se abbiamo troppi nemici vivi
        if (aliveEnemies.Count >= Mathf.Min(maxEnemiesAlive, currentWave.maxEnemiesAlive))
        {
            return;
        }
        
        // Controlla se è tempo di spawnare
        if (!lastSpawnTimes.ContainsKey(spawnData))
        {
            lastSpawnTimes[spawnData] = 0f;
        }
        
        float timeSinceLastSpawn = waveTime - lastSpawnTimes[spawnData];
        
        if (spawnData.spawnContinuously)
        {
            // Spawning continuo basato su spawnRate
            float spawnInterval = 1f / spawnData.spawnRate;
            
            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnEnemy(spawnData);
                lastSpawnTimes[spawnData] = waveTime;
            }
        }
        else
        {
            // Spawning a batch - spawn only once per wave
            if (!HasSpawnedBatch(spawnData))
            {
                StartCoroutine(SpawnBatch(spawnData));
            }
        }
    }
    
    private bool HasSpawnedBatch(SpawnData spawnData)
    {
        return spawnedBatches.Contains(spawnData);
    }
    
    private IEnumerator SpawnBatch(SpawnData spawnData)
    {
        // Marca il batch come spawnato PRIMA di iniziare
        spawnedBatches.Add(spawnData);
        
        if (debugMode)
        {
            Debug.Log($"Spawning batch: {spawnData.spawnCount} enemies of type {spawnData.enemyPrefab.name}");
        }
        
        for (int i = 0; i < spawnData.spawnCount; i++)
        {
            if (aliveEnemies.Count >= maxEnemiesAlive) break;
            
            SpawnEnemy(spawnData);
            
            if (spawnData.delayBetweenSpawns > 0 && i < spawnData.spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnData.delayBetweenSpawns);
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Batch spawn completed for {spawnData.enemyPrefab.name}");
        }
    }
    
    private void SpawnEnemy(SpawnData spawnData)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        
        if (spawnPosition == Vector3.zero)
        {
            return;
        }
        
        GameObject enemy = Instantiate(spawnData.enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Applica i moltiplicatori di difficoltà
        ApplyDifficultyScaling(enemy, spawnData);
        
        // Registra il nemico spawned
        aliveEnemies.Add(enemy);
        
        // Registra il nemico al DropManager se disponibile
        if (DropManager.Instance != null)
        {
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                DropManager.Instance.RegisterEnemy(enemyBase);
            }
        }
        
        if (debugMode)
        {
        }
    }
    
    private Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = spawnPoint.position + randomOffset;
            
            // Controlla se la posizione è abbastanza lontana dal player
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(candidate, playerTransform.position);
                if (distanceToPlayer >= minDistanceFromPlayer)
                {
                    return candidate;
                }
            }
            else
            {
                return candidate;
            }
        }
        
        return Vector3.zero; // Fallback se non trova posizione valida
    }
    
    private void ApplyDifficultyScaling(GameObject enemy, SpawnData spawnData)
    {
      
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
          enemyBase.ScaleDifficulty(spawnData.healthMultiplier, spawnData.damageMultiplier, spawnData.speedMultiplier);
            if (debugMode)
            {
            }
        }
    }
    
    private void CleanupDeadEnemies()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
    }
    
    private bool IsWaveComplete()
    {
        float waveTime = Time.time - waveStartTime;

        // Special handling for boss waves
        if (currentWave.isBossWave)
        {
            // Boss wave completes when boss is defeated
            if (bossSpawned && (currentBoss == null || currentBoss.GetComponent<HealthSystem>()?.IsAlive != true))
            {
                if (!HasInvokedBossDefeated)
                {
                    // Spawn chest drops when boss is defeated
                    if (currentBoss != null && DropManager.Instance != null)
                    {
                        // Spawn 2-3 chest drops around the boss position
                        int chestCount = Random.Range(2, 4);
                        DropManager.Instance.SpawnChestDrops(currentBoss.transform.position, chestCount, 2f);
                    }

                    OnBossDefeated?.Invoke();
                    HasInvokedBossDefeated = true;
                }
                // Also check if all other enemies are dead for boss waves
                return aliveEnemies.Count == 0;
            }
            return false;
        }

        // Regular wave completion logic
        if (waveTime >= currentWave.waveDuration)
        {
            return true;
        }

        if (currentWave.requireAllEnemiesDead)
        {
            return aliveEnemies.Count == 0 && !HasActiveSpawning(waveTime);
        }

        return false;
    }
    
    private bool HasActiveSpawning(float waveTime)
    {
        List<SpawnData> activeSpawnData = currentWave.GetActiveSpawnData(waveTime);
        return activeSpawnData.Count > 0;
    }

    private void CompleteCurrentWave()
    {
        waveActive = false;

        if (debugMode)
        {
        }

        OnWaveCompleted?.Invoke(currentWave.waveNumber);

        if (currentWave.DialoguesAtEndWave && currentWave.dialogues != null)
        {
            DialogueManager.Instance.StartDialogue(currentWave.dialogues);
        }

        // Passa alla wave successiva
        if (currentWaveIndex + 1 < waves.Count)
        {
            StartCoroutine(DelayedWaveStart(currentWaveIndex + 1, 0.1f));
        }
        else
        {
            OnAllWavesCompleted?.Invoke();
        }
        
    }
    
    private IEnumerator DelayedWaveStart(int waveIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave(waveIndex);
    }

    /// <summary>
    /// Spawns the boss for boss waves
    /// </summary>
    private void SpawnBoss()
    {
        if (currentWave.bossPrefab == null)
        {
            return;
        }

        Vector3 bossSpawnPosition = GetBossSpawnPosition();

        if (bossSpawnPosition == Vector3.zero)
        {
            return;
        }

        currentBoss = Instantiate(currentWave.bossPrefab, bossSpawnPosition, Quaternion.identity);
        bossSpawned = true;

        // Register boss to alive enemies for tracking
        aliveEnemies.Add(currentBoss);

        // Register boss to DropManager if available
        if (DropManager.Instance != null)
        {
            EnemyBase bossEnemyBase = currentBoss.GetComponent<EnemyBase>();
            if (bossEnemyBase != null)
            {
                DropManager.Instance.RegisterEnemy(bossEnemyBase);
            }
        }

        OnBossSpawned?.Invoke(currentBoss);

        if (debugMode)
        {
        }
    }

    /// <summary>
    /// Gets a suitable spawn position for the boss (center of player area)
    /// </summary>
    private Vector3 GetBossSpawnPosition()
    {
        if (playerTransform == null)
        {
            // Fallback to center of spawn points
            if (spawnPoints.Length > 0)
            {
                Vector3 center = Vector3.zero;
                foreach (Transform spawnPoint in spawnPoints)
                {
                    center += spawnPoint.position;
                }
                return center / spawnPoints.Length;
            }
            return Vector3.zero;
        }

        // Spawn boss at a good distance from player but not too far
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 bossSpawnPosition = playerTransform.position + (Vector3)(direction * 8f);

        return bossSpawnPosition;
    }

    /// <summary>
    /// Shows boss introduction text after a delay
    /// </summary>
    private IEnumerator ShowBossIntroAfterDelay(string introText, float delay)
    {
        yield return new WaitForSeconds(delay);
        OnBossIntro?.Invoke(introText);
    }
    
    // Metodi di utilità per debugging
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        
        Gizmos.color = Color.blue;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
            }
        }
        
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }
    }
    
    // Cleanup
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged.RemoveListener(OnGameManagerWaveChanged);
        }
    }
}