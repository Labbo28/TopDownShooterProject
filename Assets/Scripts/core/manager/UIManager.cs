using System;
using System.Collections;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject TextKill;
    [SerializeField] GameObject TextTime;
    [SerializeField] GameObject TextLevel;
    [SerializeField] GameObject SliderXP;
    [SerializeField] GameObject SliderPlayerHp;

    [Header("Game Over Elements")]
    [SerializeField] GameObject TextDead;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject QuitButton;

    // Rimuoviamo il campo serializzato e usiamo sempre l'istanza Singleton
    private GameManager gameManager;
    private bool eventsRegistered = false;

    private void Awake()
    {
        // Assicurati che l'UI di game over sia nascosta all'inizio
        if (TextDead != null) TextDead.SetActive(false);
        if (RetryButton != null) RetryButton.SetActive(false);
        if (QuitButton != null) QuitButton.SetActive(false);
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Aspetta che il GameManager sia pronto
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager not ready yet, retrying in 0.1 seconds");
            Invoke(nameof(InitializeUI), 0.1f);
            return;
        }

        gameManager = GameManager.Instance;
        RegisterEvents();
        UpdateAllUI();

        Debug.Log("UIManager initialized successfully");
    }

    private void RegisterEvents()
    {
        if (gameManager == null || eventsRegistered) return;

        try
        {
            gameManager.OnEnemyKilled.AddListener(OnEnemyKilled);
            gameManager.OnGameTimeChanged.AddListener(OnGameTimeChanged);
            gameManager.OnXPChanged.AddListener(OnXPChanged);
            gameManager.OnPlayerLevelUp.AddListener(OnPlayerLevelUp);
            gameManager.OnGameOver.AddListener(OnGameOver);
            Player.Instance.GetComponent<HealthSystem>().onDamaged.AddListener(OnPlayerDamaged);
            Player.Instance.GetComponent<HealthSystem>().onHealed.AddListener(OnPlayerHealed);

            if (RetryButton != null)
                RetryButton.GetComponent<Button>().onClick.AddListener(OnRetryButtonClicked);
            if (QuitButton != null)
                QuitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);

            eventsRegistered = true;
            Debug.Log("UI events registered successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to register UI events: {e.Message}");
        }
    }

    private void UpdatePlayerHealth()
    {
        if (SliderPlayerHp != null && Player.Instance != null)
        {
            Slider hpSlider = SliderPlayerHp.GetComponent<Slider>();
            HealthSystem healthSystem = Player.Instance.GetComponent<HealthSystem>();
            if (hpSlider != null && healthSystem != null)
            {
                hpSlider.value = healthSystem.Health / healthSystem.MaxHealth;
            }
            else
            {
                Debug.LogWarning("SliderPlayerHp doesn't have a Slider component or HealthSystem is missing!");
            }
        }
        else
        {
            Debug.LogWarning("SliderPlayerHp is null or Player instance is null!");
        }
    }

    private void OnPlayerDamaged()
    {
        UpdatePlayerHealth();
    }
    
    private void OnPlayerHealed()
    {
        UpdatePlayerHealth();
    }

    private void UnregisterEvents()
    {
        if (gameManager == null || !eventsRegistered) return;

        try
        {
            gameManager.OnEnemyKilled.RemoveListener(OnEnemyKilled);
            gameManager.OnGameTimeChanged.RemoveListener(OnGameTimeChanged);
            gameManager.OnXPChanged.RemoveListener(OnXPChanged);
            gameManager.OnPlayerLevelUp.RemoveListener(OnPlayerLevelUp);
            gameManager.OnGameOver.RemoveListener(OnGameOver);

            if (RetryButton != null)
                RetryButton.GetComponent<Button>().onClick.RemoveListener(OnRetryButtonClicked);
            if (QuitButton != null)
                QuitButton.GetComponent<Button>().onClick.RemoveListener(OnQuitButtonClicked);

            eventsRegistered = false;
            Debug.Log("UI events unregistered");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to unregister UI events: {e.Message}");
        }
    }

    private void UpdateAllUI()
    {
        if (gameManager == null) return;

        // Aggiorna tutti gli elementi UI con i valori correnti
        OnEnemyKilled();
        OnGameTimeChanged();
        OnXPChanged(gameManager.GetCurrentXP());
        OnPlayerLevelUp(gameManager.GetPlayerLevel());
    }

    private void OnRetryButtonClicked()
    {
        Debug.Log("Retrying...");

        // Nascondi l'UI di game over prima di ricaricare
        if (TextDead != null) TextDead.SetActive(false);
        if (RetryButton != null) RetryButton.SetActive(false);
        if (QuitButton != null) QuitButton.SetActive(false);

        // Ricarica la scena

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        Player.Instance.gameObject.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("Returning to main menu...");
        Player.Instance.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void OnGameOver()
    {
        Debug.Log("GAME OVER!");

        if (TextDead != null)
        {
            StartCoroutine(FadeInGameOver());
        }
        else
        {
            Debug.LogWarning("TextDead is null!");
        }

        if (RetryButton != null)
        {
            RetryButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("RetryButton is null!");
        }

        if (QuitButton != null)
        {
            QuitButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("QuitButton is null!");
        }
    }

    private IEnumerator FadeInGameOver()
    {
        CanvasGroup cg = TextDead.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            // Se non c'è CanvasGroup, mostra subito
            TextDead.SetActive(true);
            yield break;
        }
        
        TextDead.SetActive(true);
        cg.alpha = 0f;
        
        float duration = 2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        
        cg.alpha = 1f; // Assicura che sia completamente visibile
    }

    private void OnPlayerLevelUp(int level)
    {
        if (TextLevel != null)
        {
            Text levelText = TextLevel.GetComponent<Text>();
            if (levelText != null)
            {
                levelText.text = "Lv." + level.ToString();
            }
            else
            {
                Debug.LogWarning("TextLevel doesn't have a Text component!");
            }
        }
        else
        {
            Debug.LogWarning("TextLevel is null!");
        }
    }

    private void OnXPChanged(float xp)
    {
        if (SliderXP != null && gameManager != null)
        {
            Slider xpSlider = SliderXP.GetComponent<Slider>();
            if (xpSlider != null)
            {
                xpSlider.value = xp / gameManager.GetXPToLevelUp();
            }
            else
            {
                Debug.LogWarning("SliderXP doesn't have a Slider component!");
            }
        }
        else
        {
            Debug.LogWarning("SliderXP is null or GameManager is null!");
        }
    }

    private void OnGameTimeChanged()
    {
        if (TextTime != null && gameManager != null)
        {
            Text timeText = TextTime.GetComponent<Text>();
            if (timeText != null)
            {
                timeText.text = gameManager.getFormattedGameTime();
            }
            else
            {
                Debug.LogWarning("TextTime doesn't have a Text component!");
            }
        }
        else
        {
            Debug.LogWarning("TextTime is null or GameManager is null!");
        }
    }

    private void OnEnemyKilled()
    {
        if (TextKill != null && gameManager != null)
        {
            Text killText = TextKill.GetComponent<Text>();
            if (killText != null)
            {
                killText.text = gameManager.getEnemiesKilled().ToString();
            }
            else
            {
                Debug.LogWarning("TextKill doesn't have a Text component!");
            }
        }
        else
        {
            Debug.LogWarning("TextKill is null or GameManager is null!");
        }
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            UnregisterEvents();
        }
        else
        {
            RegisterEvents();
        }
    }
}