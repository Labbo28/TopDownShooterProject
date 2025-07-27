using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnEnemyKilled;
    public UnityEvent OnGameTimeChanged;
    public static GameManager Instance { get; private set; }

    private GameTimeDisplay gameTimeDisplay;

    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private float XP = 0;

    //attrtributi della classe MagnetDrop
    private float attractRadius = 1.5f;
    private float attractSpeed = 2f;

    //Attributi della classe Medikit_drop
    private float healAmount = 0.3f; //valore percentuale della vita ripristinata

    private int PlayerLevel = 1;
    private float xpNeededToLevelUp = 10;
    
    private float previousGameTime = 0f;
    private string formattedTime = "00:00";

    public UnityEvent<int> OnWaveChanged;
    public UnityEvent<float> OnXPChanged;
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnPlayerLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameTimeDisplay = gameObject.AddComponent<GameTimeDisplay>();

        // Initialize events
        OnEnemyKilled = new UnityEvent();
        OnGameTimeChanged = new UnityEvent();
        OnWaveChanged = new UnityEvent<int>();
        OnXPChanged = new UnityEvent<float>();
        OnGameOver = new UnityEvent();
        OnPlayerLevelUp = new UnityEvent<int>();
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (Mathf.FloorToInt(gameTime) != Mathf.FloorToInt(previousGameTime))
        {
            formattedTime = GameTimeDisplay.FormatGameTime(gameTime);
            OnGameTimeChanged?.Invoke();
        }
        
        CheckWaveProgression();
    }

    private void CheckWaveProgression()
    {
        // Logica per cambiare le ondate basata sul tempo
        int newWave = Mathf.FloorToInt(gameTime / 60f) + 1;
        
        if (newWave != currentWave)
        {
            currentWave = newWave;
            OnWaveChanged?.Invoke(currentWave);
        }
    }

    public void AddXP(float xp)
    {
        XP += xp;
        if (XP >= xpNeededToLevelUp)
        {
            PlayerLevel++;
            OnPlayerLevelUp?.Invoke(PlayerLevel);
            XP -= xpNeededToLevelUp;
            xpNeededToLevelUp *= 1.5f;
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
}