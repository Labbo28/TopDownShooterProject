using UnityEngine;

// Questa classe rappresenta i dati necessari per spawnare un tipo di nemico in una wave.
[System.Serializable]
public class SpawnData
{
    [Header("Enemy Configuration")]
    public GameObject enemyPrefab; // Prefab del nemico da spawnare
    public int spawnCount;         // Quanti nemici spawnare (per batch)
    public float spawnRate;        // Nemici per secondo (se spawnContinuously)
    public int maxEnemiesInWave;   // Numero massimo di nemici spawnabili in questa wave

    [Header("Timing")]
    public float startTime;        // Quando inizia a spawnare questo nemico nella wave
    public float duration;         // Per quanto tempo spawna (-1 = infinito)

    [Header("Spawn Behavior")]
    public bool spawnContinuously = true; // Se true, spawn rate costante; se false, tutti insieme (batch)
    public float delayBetweenSpawns = 1f; // Delay tra spawn individuali (solo per batch)

    [Header("Difficulty Scaling")]
    public float healthMultiplier = 1f;   // Moltiplicatore salute
    public float damageMultiplier = 1f;   // Moltiplicatore danno
    public float speedMultiplier = 1f;    // Moltiplicatore velocità
}