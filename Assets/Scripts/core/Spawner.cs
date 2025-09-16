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
    
    // Player reference
    private Transform playerTransform;
    
    // Events
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action OnAllWavesCompleted;
    
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
        
        // Reset spawn timers
        lastSpawnTimes.Clear();
        
        if (debugMode)
        {
        }
        
        OnWaveStarted?.Invoke(currentWave.waveNumber);
    }
    
    private void ProcessCurrentWaveSpawning()
    {
        float waveTime = Time.time - waveStartTime;
        List<SpawnData> activeSpawnData = currentWave.GetActiveSpawnData(waveTime);
        foreach (SpawnData spawnData in activeSpawnData)
        {
            ProcessSpawnData(spawnData, waveTime);
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
            // Spawning a batch
            if (timeSinceLastSpawn >= spawnData.delayBetweenSpawns && !HasSpawnedBatch(spawnData))
            {
                StartCoroutine(SpawnBatch(spawnData));
            }
        }
    }
    
    private bool HasSpawnedBatch(SpawnData spawnData)
    {
        // Logica per determinare se il batch è già stato spawnato
        // Puoi implementare questo con un Dictionary<SpawnData, bool> se necessario
        return false;
    }
    
    private IEnumerator SpawnBatch(SpawnData spawnData)
    {
        for (int i = 0; i < spawnData.spawnCount; i++)
        {
            if (aliveEnemies.Count >= maxEnemiesAlive) break;
            
            SpawnEnemy(spawnData);
            
            if (spawnData.delayBetweenSpawns > 0)
            {
                yield return new WaitForSeconds(spawnData.delayBetweenSpawns);
            }
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
        
        // Controlla se la wave è finita per tempo
        if (waveTime >= currentWave.waveDuration)
        {
            return true;
        }
        
        // Controlla se serve che tutti i nemici siano morti
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
        
        // Passa alla wave successiva
        if (currentWaveIndex + 1 < waves.Count)
        {
            StartCoroutine(DelayedWaveStart(currentWaveIndex + 1, 2f));
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