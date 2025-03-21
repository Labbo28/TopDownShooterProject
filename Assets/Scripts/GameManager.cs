using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnEnemyKilled;
    public event EventHandler OnGameTimeChanged;
    public static GameManager Instance { get; private set; }

    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private int score = 0;

    public event System.Action<int> OnWaveChanged;
    public event System.Action<int> OnScoreChanged;
    public event System.Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        CheckWaveProgression();
    }

    private void CheckWaveProgression()
    {
        // Logica per cambiare le ondate basata sul tempo
        //anche se forse sarebbe meglio una logica basata su eventi/obiettivi
        int newWave = Mathf.FloorToInt(gameTime / 60f) + 1;
        
        if (newWave != currentWave)
        {
            currentWave = newWave;
            OnWaveChanged?.Invoke(currentWave);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        // Logica aggiuntiva per quando un nemico viene ucciso
    }

    public int getEnemiesKilled()
    {
        return enemiesKilled;
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        // Logica per gestire la fine del gioco
    }
}