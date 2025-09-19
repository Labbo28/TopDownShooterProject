using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : PersistentObject
{
    // Enum per stati UI più chiari
    public enum UIState
    {
        GamePlaying,
        GamePaused,
        UpgradeMenuOpen,
        SettingsOpen,
        GameOver
    }

    [Header("UI Elements")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject TextKill;
    [SerializeField] GameObject TextTime;
    [SerializeField] GameObject TextLevel;
    [SerializeField] GameObject SliderXP;
    [SerializeField] GameObject SliderPlayerHp;
    [SerializeField] GameObject ammoImage;
    [SerializeField] GameObject ammoText;
    [SerializeField] GameObject settingsPanel;

    [Header("Game Over Elements")]
    [SerializeField] GameObject TextDead;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject QuitButton;

    [SerializeField] GameObject TextHealth;

    // Gestione stato UI
    private UIState currentUIState = UIState.GamePlaying;
    private bool wasGamePausedBeforeSettings = false;

    // Riferimenti principali
    private GameManager gameManager;
    private InputSystem_Actions inputActions;
    private bool eventsRegistered = false;
    private bool isUpgradePanelOpen = false;

    // Eventi per comunicazione stato UI
    public static event Action<UIState> OnUIStateChanged;

    protected override void Setup()
    {
        // Implementazione vuota, non necessaria per UIManager
    }
    private void Awake()
    {
        // Assicurati che l'UI di game over sia nascosta all'inizio
        if (TextDead != null) TextDead.SetActive(false);
        if (RetryButton != null) RetryButton.SetActive(false);
        if (QuitButton != null) QuitButton.SetActive(false);
    }

    private void Start()
    {
        PlayerUpgradeSystem.OnUpgradePanelClosed += OnUpgradePanelClosed;
        PlayerUpgradeSystem.OnUpgradePanelOpened += OnUpgradePanelOpened;
        InitializeUI();
        InitializeInput();
    }

    private void InitializeInput()
    {
        inputActions = new InputSystem_Actions();
        inputActions.UI.Cancel.performed += OpenSettingsPanel;
        inputActions.UI.Enable();
    }

    #region Gestione Stati UI

    private void ChangeUIState(UIState newState)
    {
        if (currentUIState != newState)
        {
            UIState previousState = currentUIState;
            currentUIState = newState;
            
            OnUIStateChanged?.Invoke(newState);
            Debug.Log($"UI State changed: {previousState} -> {newState}");
        }
    }

    public UIState GetCurrentUIState() => currentUIState;
    public bool IsGameInteractionAllowed() => currentUIState == UIState.GamePlaying;

    #endregion

    #region Gestione Pannello Settings

    private void OpenSettingsPanel(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!CanOpenSettings())
        {
            Debug.Log("Cannot open settings in current state: " + currentUIState);
            return;
        }

        if (IsSettingsOpen())
        {
            CloseSettingsPanel();
        }
        else
        {
            ShowSettingsPanel();
        }
    }

    private bool CanOpenSettings()
    {
        // Controlli di base
        if (currentUIState == UIState.GameOver)
        {
            Debug.Log("Cannot open settings: Game is over");
            return false;
        }
        
        if (gameManager == null)
        {
            Debug.LogError("Cannot open settings: GameManager is null");
            return false;
        }
        
        if (settingsPanel == null)
        {
            Debug.LogError("Cannot open settings: Settings panel is not assigned");
            return false;
        }
        
        // Verifica che il parent del settings panel sia attivo
        Transform parent = settingsPanel.transform.parent;
        while (parent != null)
        {
            if (!parent.gameObject.activeInHierarchy)
            {
                Debug.LogError($"Cannot open settings: Parent '{parent.name}' is inactive");
                return false;
            }
            parent = parent.parent;
        }
        
        return true;
    }

    private bool IsSettingsOpen()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }

    private void ShowSettingsPanel()
    {
        // Prova ad attivare il pannello
        settingsPanel.SetActive(true);
        
        // Verifica se il pannello è effettivamente attivo
        if (!settingsPanel.activeInHierarchy)
        {
            Debug.LogError("Settings panel could not be activated! Check if parent is active.");
            return;
        }
        
        // Salva lo stato corrente solo se il pannello è stato attivato con successo
        wasGamePausedBeforeSettings = (currentUIState == UIState.GamePaused || 
                                     currentUIState == UIState.UpgradeMenuOpen);
        
        // Pausa solo se necessario
        if (currentUIState == UIState.GamePlaying)
        {
            PauseGame();
        }
        
        ChangeUIState(UIState.SettingsOpen);
        Debug.Log("Settings panel opened successfully");
    }

    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        
        UIState targetState = DetermineStateAfterSettingsClosure();
        HandleGameStateAfterSettings(targetState);
        ChangeUIState(targetState);
        
        Debug.Log($"Settings panel closed, returning to state: {targetState}");
    }

    private UIState DetermineStateAfterSettingsClosure()
    {
        if (isUpgradePanelOpen)
        {
            return UIState.UpgradeMenuOpen;
        }
        
        if (gameManager?.CurrentGameState == GameState.GameOver)
        {
            return UIState.GameOver;
        }
        
        return wasGamePausedBeforeSettings ? UIState.GamePaused : UIState.GamePlaying;
    }

    private void HandleGameStateAfterSettings(UIState targetState)
    {
        switch (targetState)
        {
            case UIState.GamePlaying:
                ResumeGame();
                break;
                
            case UIState.GamePaused:
                break;
                
            case UIState.UpgradeMenuOpen:
                break;
                
            case UIState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void OnSettingsPanelClosed()
    {
        if (IsSettingsOpen())
        {
            CloseSettingsPanel();
        }
    }

    #endregion

    #region Gestione Pause/Resume

    private void PauseGame()
    {
        if (gameManager != null && gameManager.CurrentGameState != GameState.Paused)
        {
            gameManager.PauseGame();
            Time.timeScale = 0f;
            UnregisterEvents();
            Debug.Log("Game paused by UI");
        }
    }

    private void ResumeGame()
    {
        if (gameManager != null && gameManager.CurrentGameState == GameState.Paused)
        {
            gameManager.ResumeGame();
            Time.timeScale = 1f;
            RegisterEvents();
            Debug.Log("Game resumed by UI");
        }
    }

    #endregion

    #region Eventi Upgrade Panel

    private void OnUpgradePanelOpened(int level)
    {
        isUpgradePanelOpen = true;
        if (!IsSettingsOpen())
        {
            ChangeUIState(UIState.UpgradeMenuOpen);
        }
    }

    private void OnUpgradePanelClosed(int level)
    {
        isUpgradePanelOpen = false;
        
        if (!IsSettingsOpen())
        {
            ChangeUIState(UIState.GamePlaying);
        }
    }

    #endregion

    #region Inizializzazione UI

    private void InitializeUI()
    {
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

            if (Player.Instance != null)
            {
                var healthSystem = Player.Instance.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.onDamaged.AddListener(OnPlayerDamaged);
                    healthSystem.onHealed.AddListener(OnPlayerHealed);
                }

                // Gestione più robusta per l'arma
                RegisterWeaponEvents();
            }

            // Eventi bottoni
            RegisterButtonEvents();

            eventsRegistered = true;
            Debug.Log("UI Events registered successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in RegisterEvents: {e.Message}");
        }
    }

    private void RegisterWeaponEvents()
    {
        try
        {
            var weaponHolder = Player.Instance?.transform.Find("Weapons");
            if (weaponHolder != null)
            {
                var assaultRifle = weaponHolder.Find("Assault_Rifle");
                if (assaultRifle != null)
                {
                    var weapon = assaultRifle.GetComponent<Weapon>();
                    if (weapon != null)
                    {
                        weapon.OnAmmoChanged.AddListener(OnAmmoChanged);
                        Debug.Log("Weapon events registered");
                    }
                    else
                    {
                        Debug.LogWarning("Weapon component not found on Assault_Rifle");
                    }
                }
                else
                {
                    Debug.LogWarning("Assault_Rifle not found in Weapons");
                }
            }
            else
            {
                Debug.LogWarning("Weapons holder not found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error registering weapon events: {e.Message}");
        }
    }

    private void RegisterButtonEvents()
    {
        try
        {
            if (RetryButton != null)
            {
                var retryBtn = RetryButton.GetComponent<Button>();
                if (retryBtn != null)
                {
                    retryBtn.onClick.AddListener(OnRetryButtonClicked);
                }
            }

            if (QuitButton != null)
            {
                var quitBtn = QuitButton.GetComponent<Button>();
                if (quitBtn != null)
                {
                    quitBtn.onClick.AddListener(OnQuitButtonClicked);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error registering button events: {e.Message}");
        }
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

            if (Player.Instance != null)
            {
                var healthSystem = Player.Instance.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.onDamaged.RemoveListener(OnPlayerDamaged);
                    healthSystem.onHealed.RemoveListener(OnPlayerHealed);
                }
            }

            // Rimuovi eventi bottoni
            UnregisterButtonEvents();

            eventsRegistered = false;
            Debug.Log("UI Events unregistered successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in UnregisterEvents: {e.Message}");
        }
    }

    private void UnregisterButtonEvents()
    {
        try
        {
            if (RetryButton != null)
            {
                var retryBtn = RetryButton.GetComponent<Button>();
                retryBtn?.onClick.RemoveListener(OnRetryButtonClicked);
            }

            if (QuitButton != null)
            {
                var quitBtn = QuitButton.GetComponent<Button>();
                quitBtn?.onClick.RemoveListener(OnQuitButtonClicked);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error unregistering button events: {e.Message}");
        }
    }

    #endregion

    #region Aggiornamenti UI

    private void UpdateAllUI()
    {
        if (gameManager?.CurrentGameState == GameState.GameOver) return;
        if (gameManager == null) return;

        OnEnemyKilled();
        OnGameTimeChanged();
        OnXPChanged(gameManager.GetCurrentXP());
        OnPlayerLevelUp(gameManager.GetPlayerLevel());
        UpdatePlayerHealth();
    }

    private void UpdatePlayerHealth()
    {
        if (SliderPlayerHp == null || Player.Instance == null) return;

        var hpSlider = SliderPlayerHp.GetComponent<Slider>();
        var healthSystem = Player.Instance.GetComponent<HealthSystem>();

        if (hpSlider == null || healthSystem == null) return;

        hpSlider.value = healthSystem.Health / healthSystem.MaxHealth;

        if (TextHealth != null)
        {
            var healthText = TextHealth.GetComponent<Text>();
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(healthSystem.Health)}/{Mathf.CeilToInt(healthSystem.MaxHealth)}";
            }
        }
    }

    private void OnAmmoChanged(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            var ammoTextComponent = ammoText.GetComponent<TextMeshProUGUI>();
            if (ammoTextComponent != null)
            {
                ammoTextComponent.text = $"{currentAmmo}/{maxAmmo}";
            }
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

    private void OnPlayerLevelUp(int level)
    {
        AudioManager.Instance?.PlayLevelUpSound();
        if (TextLevel != null)
        {
            var levelText = TextLevel.GetComponent<Text>();
            if (levelText != null)
            {
                levelText.text = "Lv." + level.ToString();
            }
            else
            {
                Debug.LogWarning("Text component not found on TextLevel");
            }
        }
        else
        {
            Debug.LogWarning("TextLevel is null");
        }
    }

    private void OnXPChanged(float xp)
    {
        if (SliderXP != null && gameManager != null)
        {
            var xpSlider = SliderXP.GetComponent<Slider>();
            if (xpSlider != null)
            {
                xpSlider.value = xp / gameManager.GetXPToLevelUp();
            }
            else
            {
                Debug.LogWarning("Slider component not found on SliderXP");
            }
        }
        else
        {
            Debug.LogWarning("SliderXP or gameManager is null");
        }
    }

    private void OnGameTimeChanged()
    {
        if (TextTime != null && gameManager != null)
        {
            var timeText = TextTime.GetComponent<Text>();
            if (timeText != null)
            {
                timeText.text = gameManager.getFormattedGameTime();
            }
            else
            {
                Debug.LogWarning("Text component not found on TextTime");
            }
        }
        else
        {
            Debug.LogWarning("TextTime or gameManager is null");
        }
    }

    private void OnEnemyKilled()
    {
        if (TextKill != null && gameManager != null)
        {
            var killText = TextKill.GetComponent<Text>();
            if (killText != null)
            {
                killText.text = gameManager.getEnemiesKilled().ToString();
            }
            else
            {
                Debug.LogWarning("Text component not found on TextKill");
            }
        }
        else
        {
            Debug.LogWarning("TextKill or gameManager is null");
        }
    }

    #endregion

    #region Game Over

    public void DisableUIElements()
    {
        if (TextKill != null) TextKill.SetActive(false);
        if (TextTime != null) TextTime.SetActive(false);
        if (TextLevel != null) TextLevel.SetActive(false);
        if (SliderXP != null) SliderXP.SetActive(false);
        if (SliderPlayerHp != null) SliderPlayerHp.SetActive(false);
        if (TextHealth != null) TextHealth.SetActive(false);
        if (ammoText != null) ammoText.SetActive(false);
        if (ammoImage != null) ammoImage.SetActive(false);
    }

    private void OnGameOver()
    {
        ChangeUIState(UIState.GameOver);
        DisableUIElements();
        AudioManager.Instance?.PlayLooseSound();
        
        if (TextDead != null)
        {
            StartCoroutine(FadeInGameOver());
        }
        else
        {
            Debug.LogWarning("TextDead is null");
        }

        if (RetryButton != null)
        {
            RetryButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("RetryButton is null");
        }

        if (QuitButton != null)
        {
            QuitButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("QuitButton is null");
        }
    }

    private IEnumerator FadeInGameOver()
    {
        var cg = TextDead.GetComponent<CanvasGroup>();
        if (cg == null)
        {
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
        
        cg.alpha = 1f;
    }

    #endregion

    #region Eventi Bottoni

    private void OnRetryButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();

        // Nascondi l'UI di game over
        if (TextDead != null) TextDead.SetActive(false);
        if (RetryButton != null) RetryButton.SetActive(false);
        if (QuitButton != null) QuitButton.SetActive(false);

        // Reset time scale e ricarica scena
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        
        // Rimuovi la linea problematica che accedeva a Player.Instance dopo scene reload
        Debug.Log("Scene reloaded for retry");
    }

    public void OnQuitButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;
        
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.SetActive(false);
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Debug e Validazione

    [ContextMenu("Validate UI State")]
    private void ValidateUIState()
    {
        bool settingsActive = IsSettingsOpen();
        bool upgradeActive = isUpgradePanelOpen;
        bool gameOver = gameManager?.CurrentGameState == GameState.GameOver;

        Debug.Log($"=== UI State Validation ===");
        Debug.Log($"Current State: {currentUIState}");
        Debug.Log($"Settings Active: {settingsActive}");
        Debug.Log($"Upgrade Active: {upgradeActive}");
        Debug.Log($"Game Over: {gameOver}");
        Debug.Log($"Time Scale: {Time.timeScale}");
        Debug.Log($"Events Registered: {eventsRegistered}");
        
        // Controlla consistenza
        if (currentUIState == UIState.SettingsOpen && !settingsActive)
        {
            Debug.LogWarning("State inconsistency: SettingsOpen but panel not active!");
        }
        
        if (currentUIState == UIState.UpgradeMenuOpen && !upgradeActive)
        {
            Debug.LogWarning("State inconsistency: UpgradeMenuOpen but panel not active!");
        }

        if (currentUIState == UIState.GameOver && !gameOver)
        {
            Debug.LogWarning("State inconsistency: UI thinks game is over but GameManager doesn't!");
        }
    }

    [ContextMenu("Debug Settings Panel")]
    private void DebugSettingsPanel()
    {
        Debug.Log($"=== Settings Panel Debug ===");
        Debug.Log($"Settings Panel assigned: {settingsPanel != null}");
        
        if (settingsPanel != null)
        {
            Debug.Log($"Panel active: {settingsPanel.activeSelf}");
            Debug.Log($"Panel activeInHierarchy: {settingsPanel.activeInHierarchy}");
            Debug.Log($"Panel name: {settingsPanel.name}");
            
            // Controlla la gerarchia dei parent
            Transform parent = settingsPanel.transform.parent;
            int level = 0;
            while (parent != null && level < 5)
            {
                Debug.Log($"Parent {level}: {parent.name} (active: {parent.gameObject.activeInHierarchy})");
                parent = parent.parent;
                level++;
            }
            
            // Controlla Canvas
            Canvas parentCanvas = settingsPanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                Debug.Log($"Parent Canvas: {parentCanvas.name} (enabled: {parentCanvas.enabled})");
            }
            else
            {
                Debug.LogWarning("No Canvas found in parents!");
            }
        }
        
        Debug.Log($"Can open settings: {CanOpenSettings()}");
        Debug.Log($"Is settings open: {IsSettingsOpen()}");
    }

    [ContextMenu("Force UI Refresh")]
    private void ForceUIRefresh()
    {
        UpdateAllUI();
        Debug.Log("UI manually refreshed");
    }

    #endregion

    #region Unity Lifecycle

    private void OnDestroy()
    {
        UnregisterEvents();
        
        if (inputActions != null)
        {
            inputActions.UI.Cancel.performed -= OpenSettingsPanel;
            inputActions.UI.Disable();
            inputActions.Dispose();
        }
        
        PlayerUpgradeSystem.OnUpgradePanelClosed -= OnUpgradePanelClosed;
        PlayerUpgradeSystem.OnUpgradePanelOpened -= OnUpgradePanelOpened;
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

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentUIState == UIState.GamePlaying)
        {
            // Auto-pausa quando l'app perde il focus
            ShowSettingsPanel();
        }
    }

    #endregion
}