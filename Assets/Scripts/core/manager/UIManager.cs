using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] GameObject ammoImage;
    [SerializeField] GameObject ammoText;

    [Header("Game Over Elements")]
    [SerializeField] GameObject TextDead;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject QuitButton;

    [SerializeField] GameObject TextHealth;

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
            Invoke(nameof(InitializeUI), 0.1f);
            return;
        }

        gameManager = GameManager.Instance;
        RegisterEvents();
        UpdateAllUI();

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

            //damn this shit is crazy
            Player.Instance.transform.Find("Weapons").transform.Find("Assault_Rifle").GetComponent<Weapon>().OnAmmoChanged.AddListener(OnAmmoChanged);

            if (RetryButton != null)
                RetryButton.GetComponent<Button>().onClick.AddListener(OnRetryButtonClicked);
            if (QuitButton != null)
                QuitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);

            eventsRegistered = true;
        }
        catch (System.Exception e)
        {
        }
    }
    
    /// <summary>
    /// Disabilita tutti gli elementi UI principali (non quelli di Game Over).
    /// </summary>
    public void DisableUIElements()
    {
        if (TextKill != null) TextKill.SetActive(false);
        if (TextTime != null) TextTime.SetActive(false);
        if (TextLevel != null) TextLevel.SetActive(false);
        if (SliderXP != null) SliderXP.SetActive(false);
        if (SliderPlayerHp != null) SliderPlayerHp.SetActive(false);
        if (TextHealth != null) TextHealth.SetActive(false);
        
    }

    private void UpdatePlayerHealth()
    {
        if (SliderPlayerHp == null || Player.Instance == null) return;

        Slider hpSlider = SliderPlayerHp.GetComponent<Slider>();
        HealthSystem healthSystem = Player.Instance.GetComponent<HealthSystem>();

        if (hpSlider == null || healthSystem == null) return;

        // Corretto: usa le proprietà invece dei metodi inesistenti
        hpSlider.value = healthSystem.Health / healthSystem.MaxHealth;

        if (TextHealth != null)
        {
            Text healthText = TextHealth.GetComponent<Text>();
            if (healthText != null)
            {
                // Corretto: usa Health invece di GetCurrentHealth e MaxHealth invece di maxHealth
                healthText.text = $"{Mathf.CeilToInt(healthSystem.Health)}/{Mathf.CeilToInt(healthSystem.MaxHealth)}";
            }
        }
    }

  private void OnAmmoChanged(int currentAmmo, int maxAmmo)
    {
        
        if (ammoText != null)
        {
            ammoText.GetComponent<TextMeshProUGUI>().text = $"{currentAmmo}/{maxAmmo}";
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
        }
        catch (System.Exception e)
        {
        }
    }

    private void UpdateAllUI()
    {
        if(GameManager.Instance.CurrentGameState==GameState.GameOver) return;
        if (gameManager == null) return;

        // Aggiorna tutti gli elementi UI con i valori correnti
        OnEnemyKilled();
        OnGameTimeChanged();
        OnXPChanged(gameManager.GetCurrentXP());
        OnPlayerLevelUp(gameManager.GetPlayerLevel());
        UpdatePlayerHealth();
    }

    private void OnRetryButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();

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
        AudioManager.Instance?.PlayButtonClick();
        Player.Instance.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void OnGameOver()
    {
        DisableUIElements();
        AudioManager.Instance?.PlayLooseSound();
        if (TextDead != null)
        {
            StartCoroutine(FadeInGameOver());
        }
        else
        {
        }

        if (RetryButton != null)
        {
            RetryButton.SetActive(true);
        }
        else
        {
        }

        if (QuitButton != null)
        {
            QuitButton.SetActive(true);
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


    private void OnAmmoChanged(int currentAmmo)
    {
        // Implementa l'aggiornamento dell'UI per l'ammo se necessario
    }
    private void OnPlayerLevelUp(int level)
    {
        AudioManager.Instance?.PlayLevelUpSound();
        if (TextLevel != null)
        {
            Text levelText = TextLevel.GetComponent<Text>();
            if (levelText != null)
            {
                levelText.text = "Lv." + level.ToString();
            }
            else
            {
            }
        }
        else
        {
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
            }
        }
        else
        {
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
            }
        }
        else
        {
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
            }
        }
        else
        {
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