using UnityEngine;

[System.Serializable]
public class SpawnData
{
    [Header("Enemy Configuration")]
    public GameObject enemyPrefab;
    public int spawnCount;
    public float spawnRate; // nemici per secondo
    
    [Header("Timing")]
    public float startTime; // quando inizia a spawnare questo nemico nella wave
    public float duration; // per quanto tempo spawna (-1 = infinito)
    
    [Header("Spawn Behavior")]
    public bool spawnContinuously = true; // se false, spawna tutti insieme
    public float delayBetweenSpawns = 1f; // delay tra spawn individuali
    
    [Header("Difficulty Scaling")]
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
}