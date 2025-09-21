using UnityEngine;
using System.Collections.Generic;

// ScriptableObject che rappresenta una singola wave di nemici.
[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave", order = 3)]
public class Wave : ScriptableObject
{
    [Header("Wave Information")]
    public int waveNumber;             // Numero identificativo della wave
    public string waveName;            // Nome della wave
    public float waveDuration = 60f;   // Durata della wave in secondi

    [Header("Spawn Configuration")]
    public List<SpawnData> spawnData = new List<SpawnData>(); // Lista dei tipi di nemici da spawnare

    [Header("Wave Conditions")]
    public bool requireAllEnemiesDead = false; // Se true, la wave termina solo quando tutti i nemici sono morti
    public int maxEnemiesAlive = 50;           // Limite massimo di nemici vivi contemporaneamente

    [Header("Rewards and Difficulty")]
    public float xpMultiplier = 1f;            // Moltiplicatore XP
    public float dropRateMultiplier = 1f;      // Moltiplicatore drop

    [Header("Boss Wave")]
    public bool isBossWave = false;            // Se è una boss wave
    public GameObject bossPrefab;              // Prefab del boss

    [Header("Boss Wave Settings")]
    [SerializeField] private float bossSpawnDelay = 3f; // Ritardo prima dello spawn del boss
    [SerializeField] private bool showBossIntro = true; // Mostra intro boss
    [SerializeField] private string bossIntroText = ""; // Testo intro boss
    [SerializeField] private bool pauseRegularSpawningDuringBoss = false; // Pausa spawn normali durante il boss
    [SerializeField] public Dialogue dialogues; // Dialoghi associati alla wave

    [SerializeField] public bool DialoguesAtEndWave; // Se mostrare dialoghi a fine wave

    // Restituisce tutti i tipi di nemici presenti nella wave
    public List<GameObject> GetAllEnemyTypes()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach (SpawnData data in spawnData)
        {
            if (data.enemyPrefab != null && !enemies.Contains(data.enemyPrefab))
            {
                enemies.Add(data.enemyPrefab);
            }
        }
        return enemies;
    }

    // Restituisce i dati di spawn attivi in base al tempo corrente della wave
    public List<SpawnData> GetActiveSpawnData(float currentTime)
    {
        List<SpawnData> activeData = new List<SpawnData>();
        foreach (SpawnData data in spawnData)
        {
            bool hasStarted = currentTime >= data.startTime;
            bool hasEnded = data.duration > 0 && currentTime >= (data.startTime + data.duration);
            if (hasStarted && !hasEnded)
            {
                activeData.Add(data);
            }
        }
        return activeData;
    }

    // Proprietà di accesso per i parametri boss
    public float BossSpawnDelay => bossSpawnDelay;
    public bool ShowBossIntro => showBossIntro;
    public string BossIntroText => string.IsNullOrEmpty(bossIntroText) ? $"{waveName} approaches!" : bossIntroText;
    public bool PauseRegularSpawningDuringBoss => pauseRegularSpawningDuringBoss;
}