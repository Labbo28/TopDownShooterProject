using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave", order = 3)]
public class Wave : ScriptableObject
{
    [Header("Wave Information")]
    public int waveNumber;
    public string waveName;
    public float waveDuration = 60f; // durata della wave in secondi
    
    [Header("Spawn Configuration")]
    public List<SpawnData> spawnData = new List<SpawnData>();
    
    [Header("Wave Conditions")]
    public bool requireAllEnemiesDead = false; // se true, la wave finisce solo quando tutti i nemici sono morti
    public int maxEnemiesAlive = 50; // massimo numero di nemici vivi contemporaneamente
    
    [Header("Rewards and Difficulty")]
    public float xpMultiplier = 1f;
    public float dropRateMultiplier = 1f;
    
    [Header("Boss Wave")]
    public bool isBossWave = false;
    public GameObject bossPrefab;
    
    // Metodo per ottenere tutti i tipi di nemici in questa wave
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
    
    // Ottieni spawn data per un tempo specifico
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
}