using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
   
    public event EventHandler OnEnemyKilled;
    public event EventHandler OnGameTimeChanged;
    public static GameManager Instance { get; private set; }

    private GameTimeDisplay gameTimeDisplay;

    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private float XP = 0;

    private int  PlayerLevel = 1;
    private float xpNeededToLevelUp = 10;
    
    private float previousGameTime = 0f;
    private string formattedTime = "00:00";

    public event System.Action<int> OnWaveChanged;
    public event System.Action<float> OnXPChanged;
    public event System.Action OnGameOver;
    public event System.Action<int> OnPlayerLevelUp;

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
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if(Mathf.FloorToInt(gameTime) != Mathf.FloorToInt(previousGameTime))
        {
            formattedTime = GameTimeDisplay.FormatGameTime(gameTime);
            OnGameTimeChanged?.Invoke(this, EventArgs.Empty);
        }
        
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


    public void AddXP(float xp)
    {
        XP += xp;
        if (XP >= xpNeededToLevelUp)
        {
            PlayerLevel++;
            OnPlayerLevelUp.Invoke(PlayerLevel);
            XP -= xpNeededToLevelUp;
            xpNeededToLevelUp *= 1.5f;
        }
        OnXPChanged?.Invoke(XP);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        OnEnemyKilled?.Invoke(this, EventArgs.Empty);
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
}