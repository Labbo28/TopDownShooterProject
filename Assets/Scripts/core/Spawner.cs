using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Gestisce lo spawn dei nemici e delle wave.
public class Spawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<Wave> waves = new List<Wave>(); // Lista delle wave da gestire
    [SerializeField] private Transform[] spawnPoints;             // Punti di spawn possibili
    [SerializeField] private float spawnRadius = 1f;              // Raggio casuale attorno al punto di spawn

    [Header("Spawn Limits")]
    [SerializeField] private int maxEnemiesAlive = 50;            // Limite massimo di nemici vivi

    [SerializeField] private float minDistanceFromPlayer = 5f;    // Distanza minima dal player per spawnare

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;              // Abilita log di debug

    // Stato corrente della wave
    private int currentWaveIndex = 0;
    private Wave currentWave;
    private float waveStartTime;
    private bool waveActive = false;

    // Tracciamento nemici
    private List<GameObject> aliveEnemies = new List<GameObject>(); // Nemici vivi
    private Dictionary<SpawnData, float> lastSpawnTimes = new Dictionary<SpawnData, float>(); // Ultimo spawn per tipo
    private HashSet<SpawnData> spawnedBatches = new HashSet<SpawnData>(); // Batch già spawnati
    private Dictionary<SpawnData, int> spawnedCountPerType = new Dictionary<SpawnData, int>(); // <--- AGGIUNTO

    // Boss tracking
    private GameObject currentBoss;
    private bool bossSpawned = false;
    private bool HasInvokedBossDefeated = false;

    // Player reference
    private Transform playerTransform;

    // Eventi pubblici per comunicare con altri sistemi
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action OnAllWavesCompleted;
    public System.Action<GameObject> OnBossSpawned;
    public System.Action OnBossWaveStarted;
    public System.Action OnBossDefeated;
    public System.Action<string> OnBossIntro;

    // Trova il player nella scena
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
                Invoke(nameof(FindPlayer), 0.1f); // Riprova dopo poco
            }
        }
    }

    // Inizializzazione
    private void Start()
    {
        playerTransform = FindObjectOfType<Player>()?.transform;
        if (playerTransform == null) return;

        // Si collega agli eventi del GameManager per sincronizzare le wave
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged.AddListener(OnGameManagerWaveChanged);
        }

        InitializeSpawner();
    }

    // Avvia la prima wave se presente
    private void InitializeSpawner()
    {
        if (waves.Count > 0)
        {
            StartWave(0);
        }
    }

    // Aggiornamento ad ogni frame
    private void Update()
    {
        if (!waveActive || currentWave == null ||
            GameManager.Instance.CurrentGameState == GameState.GameOver) return;

        CleanupDeadEnemies(); // Rimuove i nemici morti dalla lista

        if (IsWaveComplete())
        {
            CompleteCurrentWave();
            return;
        }

        ProcessCurrentWaveSpawning(); // Gestisce lo spawn dei nemici
    }

    // Gestisce il cambio wave richiesto dal GameManager
    private void OnGameManagerWaveChanged(int newWave)
    {
        if (newWave - 1 != currentWaveIndex && newWave - 1 < waves.Count)
        {
            StartWave(newWave - 1);
        }
    }

    // Avvia una wave specifica
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

        // Reset stato spawn
        lastSpawnTimes.Clear();
        spawnedBatches.Clear();
        spawnedCountPerType.Clear(); // <--- AGGIUNTO
        bossSpawned = false;
        currentBoss = null;
        HasInvokedBossDefeated = false;

        if (currentWave.isBossWave)
        {
            OnBossWaveStarted?.Invoke();
            if (currentWave.ShowBossIntro)
            {
                StartCoroutine(ShowBossIntroAfterDelay(currentWave.BossIntroText, 1f));
            }
        }

        OnWaveStarted?.Invoke(currentWave.waveNumber);
    }

    // Gestisce lo spawn dei nemici della wave corrente
    private void ProcessCurrentWaveSpawning()
    {
        float waveTime = Time.time - waveStartTime;

        // Gestione boss
        if (currentWave.isBossWave && !bossSpawned && waveTime >= currentWave.BossSpawnDelay)
        {
            SpawnBoss();
        }

        // Se il boss è vivo e la wave lo richiede, pausa lo spawn dei nemici normali
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

    // Gestisce lo spawn di un singolo tipo di nemico
    private void ProcessSpawnData(SpawnData spawnData, float waveTime)
    {
        if (aliveEnemies.Count >= Mathf.Min(maxEnemiesAlive, currentWave.maxEnemiesAlive))
        {
            return; // Troppi nemici vivi
        }

        if (!spawnedCountPerType.ContainsKey(spawnData))
            spawnedCountPerType[spawnData] = 0;

        // --- Limite massimo per tipo ---
        if (spawnData.maxEnemiesInWave > 0 && spawnedCountPerType[spawnData] >= spawnData.maxEnemiesInWave)
        {
            Debug.Log($"[SPAWN BLOCCATO] {spawnData.enemyPrefab.name}: raggiunto il limite ({spawnedCountPerType[spawnData]}/{spawnData.maxEnemiesInWave})");
            return;
        }

        if (!lastSpawnTimes.ContainsKey(spawnData))
        {
            lastSpawnTimes[spawnData] = 0f;
        }

        float timeSinceLastSpawn = waveTime - lastSpawnTimes[spawnData];

        if (spawnData.spawnContinuously)
        {
            float spawnInterval = 1f / spawnData.spawnRate;
            if (timeSinceLastSpawn >= spawnInterval)
            {
                if (spawnData.maxEnemiesInWave > 0 && spawnedCountPerType[spawnData] >= spawnData.maxEnemiesInWave)
                {
                    Debug.Log($"[SPAWN CONTINUO BLOCCATO] {spawnData.enemyPrefab.name}: raggiunto il limite ({spawnedCountPerType[spawnData]}/{spawnData.maxEnemiesInWave})");
                    return;
                }
                Debug.Log($"[SPAWN CONTINUO] {spawnData.enemyPrefab.name}: spawn n° {spawnedCountPerType[spawnData]+1} (limite {spawnData.maxEnemiesInWave})");
                SpawnEnemy(spawnData);
                lastSpawnTimes[spawnData] = waveTime;
            }
        }
        else
        {
            if (!HasSpawnedBatch(spawnData) && 
                (spawnData.maxEnemiesInWave <= 0 || spawnedCountPerType[spawnData] < spawnData.maxEnemiesInWave))
            {
                Debug.Log($"[BATCH SPAWN] {spawnData.enemyPrefab.name}: batch da {spawnData.spawnCount}, già spawnati {spawnedCountPerType[spawnData]} (limite {spawnData.maxEnemiesInWave})");
                spawnedBatches.Add(spawnData);
                StartCoroutine(SpawnBatch(spawnData));
            }
        }
    }

    // Controlla se un batch è già stato spawnato
    private bool HasSpawnedBatch(SpawnData spawnData)
    {
        return spawnedBatches.Contains(spawnData);
    }

    // Coroutine per spawnare un batch di nemici
    private IEnumerator SpawnBatch(SpawnData spawnData)
    {
        for (int i = 0; i < spawnData.spawnCount; i++)
        {
            if (aliveEnemies.Count >= maxEnemiesAlive) break;

            if (!spawnedCountPerType.ContainsKey(spawnData))
                spawnedCountPerType[spawnData] = 0;
            if (spawnData.maxEnemiesInWave > 0 && spawnedCountPerType[spawnData] >= spawnData.maxEnemiesInWave)
            {
                Debug.Log($"[BATCH BLOCCATO] {spawnData.enemyPrefab.name}: raggiunto il limite ({spawnedCountPerType[spawnData]}/{spawnData.maxEnemiesInWave})");
                break;
            }

            Debug.Log($"[BATCH SPAWN] {spawnData.enemyPrefab.name}: spawn n° {spawnedCountPerType[spawnData]+1} (limite {spawnData.maxEnemiesInWave})");
            SpawnEnemy(spawnData);
            if (spawnData.delayBetweenSpawns > 0 && i < spawnData.spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnData.delayBetweenSpawns);
            }
        }
    }

    // Spawna un singolo nemico
    private void SpawnEnemy(SpawnData spawnData)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition == Vector3.zero) return;

        GameObject enemy = Instantiate(spawnData.enemyPrefab, spawnPosition, Quaternion.identity);
        ApplyDifficultyScaling(enemy, spawnData);
        aliveEnemies.Add(enemy);

        if (!spawnedCountPerType.ContainsKey(spawnData))
            spawnedCountPerType[spawnData] = 0;
        spawnedCountPerType[spawnData]++;

        Debug.Log($"[SPAWN EFFETTUATO] {spawnData.enemyPrefab.name}: totale spawnati {spawnedCountPerType[spawnData]} (limite {spawnData.maxEnemiesInWave})");

        if (DropManager.Instance != null)
        {
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                DropManager.Instance.RegisterEnemy(enemyBase);
            }
        }
    }

    // Trova una posizione valida per spawnare (lontana dal player)
    private Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = spawnPoint.position + randomOffset;
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
        return Vector3.zero; // Nessuna posizione valida trovata
    }

    // Applica i moltiplicatori di difficoltà al nemico
    private void ApplyDifficultyScaling(GameObject enemy, SpawnData spawnData)
    {
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.ScaleDifficulty(spawnData.healthMultiplier, spawnData.damageMultiplier, spawnData.speedMultiplier);
        }
    }

    // Rimuove i nemici morti dalla lista
    private void CleanupDeadEnemies()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
    }

    // Controlla se la wave è completa
    private bool IsWaveComplete()
    {
        float waveTime = Time.time - waveStartTime;

        // Boss wave: termina solo quando il boss è morto e non ci sono altri nemici
        if (currentWave.isBossWave)
        {
            if (bossSpawned && (currentBoss == null || currentBoss.GetComponent<HealthSystem>()?.IsAlive != true))
            {
                if (!HasInvokedBossDefeated)
                {
                    // Drop chest quando il boss muore
                    if (currentBoss != null && DropManager.Instance != null)
                    {
                        int chestCount = Random.Range(2, 4);
                        DropManager.Instance.SpawnChestDrops(currentBoss.transform.position, chestCount, 2f);
                    }
                    OnBossDefeated?.Invoke();
                    HasInvokedBossDefeated = true;
                }
                return aliveEnemies.Count == 0;
            }
            return false;
        }

        // Wave normale: termina per tempo o per nemici morti (se richiesto)
        if (waveTime >= currentWave.waveDuration)
        {
            return true;
        }

        return false;
    }

    // Controlla se ci sono ancora spawn attivi
    private bool HasActiveSpawning(float waveTime)
    {
        List<SpawnData> activeSpawnData = currentWave.GetActiveSpawnData(waveTime);
        return activeSpawnData.Count > 0;
    }

    // Termina la wave corrente e avvia la successiva dopo un delay
    private void CompleteCurrentWave()
    {
        waveActive = false;
        OnWaveCompleted?.Invoke(currentWave.waveNumber);

        if(currentWave.DialoguesAtEndWave && currentWave.dialogues != null)
        {
            DialogueManager.Instance.StartDialogue(currentWave.dialogues);
        }

        if (currentWaveIndex + 1 < waves.Count)
        {
            StartCoroutine(DelayedWaveStart(currentWaveIndex + 1, 10f));
        }
        else
        {
            OnAllWavesCompleted?.Invoke();
        }
    }

    // Avvia la wave dopo un certo delay
    private IEnumerator DelayedWaveStart(int waveIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave(waveIndex);
    }

    // Spawna il boss
    private void SpawnBoss()
    {
        if (currentWave.bossPrefab == null) return;
        Vector3 bossSpawnPosition = GetBossSpawnPosition();
        if (bossSpawnPosition == Vector3.zero) return;

        currentBoss = Instantiate(currentWave.bossPrefab, bossSpawnPosition, Quaternion.identity);
        bossSpawned = true;
        aliveEnemies.Add(currentBoss);

        if (DropManager.Instance != null)
        {
            EnemyBase bossEnemyBase = currentBoss.GetComponent<EnemyBase>();
            if (bossEnemyBase != null)
            {
                DropManager.Instance.RegisterEnemy(bossEnemyBase);
            }
        }
        OnBossSpawned?.Invoke(currentBoss);
    }

    // Trova una posizione adatta per il boss
    private Vector3 GetBossSpawnPosition()
    {
        if (playerTransform == null)
        {
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
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 bossSpawnPosition = playerTransform.position + (Vector3)(direction * 8f);
        return bossSpawnPosition;
    }

    // Mostra l'intro del boss dopo un delay
    private IEnumerator ShowBossIntroAfterDelay(string introText, float delay)
    {
        yield return new WaitForSeconds(delay);
        OnBossIntro?.Invoke(introText);
    }

    // Gizmo per debug visuale in editor
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

    // Cleanup eventi
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged.RemoveListener(OnGameManagerWaveChanged);
        }
    }
}