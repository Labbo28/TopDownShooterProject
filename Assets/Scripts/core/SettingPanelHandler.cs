using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingPanelHandler : MonoBehaviour 
{
    [SerializeField] private Button quitPanelButton;
    [SerializeField] private Button muteMainButton;
    [SerializeField] private Button muteEffectsButton;
    [SerializeField] private Button muteMusicButton;

    [SerializeField] private GameObject optionButtonGameObject;
  
    [SerializeField] private Slider sliderMainVolume;
    [SerializeField] private Slider sliderEffectsVolume;
    [SerializeField] private Slider sliderMusicVolume;

    [SerializeField] private Image mainVolumeImage;
    [SerializeField] private Image effectsVolumeImage;
    [SerializeField] private Image musicVolumeImage;
    
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmutedSprite;
    
    private bool isMainMuted = false;
    private bool isEffectsMuted = false;
    private bool isMusicMuted = false;
    
    private float lastMainVolume = 1f;
    private float lastEffectsVolume = 1f;
    private float lastMusicVolume = 1f;
    
    private bool hasBeenInitialized = false;
    
    void Awake()
    {
        // RIMOSSA la linea problematica: gameObject.SetActive(false);
        // Il pannello deve rimanere nello stato in cui è stato configurato nell'editor
        
        // Carica sprite se necessario
        if (mutedSprite == null)
            mutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/muted.png");

        if (unmutedSprite == null)
            unmutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/unmuted.png");
            
        Debug.Log($"SettingPanelHandler Awake - Panel active: {gameObject.activeSelf}");
    }

    void OnEnable()
    {
        // Inizializza solo la prima volta che il pannello viene attivato
        if (!hasBeenInitialized)
        {
            InitializePanel();
            hasBeenInitialized = true;
        }
        
        Debug.Log("Settings panel enabled");
    }

    void OnDisable()
    {
        Debug.Log("Settings panel disabled");
        
        // Se siamo in GameScene e il pannello viene disattivato, notifica l'UIManager
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.OnSettingsPanelClosed();
            }
        }
    }
    
    void Start()
    {
        // Inizializza se non è già stato fatto (nel caso il pannello sia attivo dall'inizio)
        if (!hasBeenInitialized)
        {
            InitializePanel();
            hasBeenInitialized = true;
        }
    }
    
    private void InitializePanel()
    {
        Debug.Log("Initializing settings panel...");
        
        // Configurazione bottoni basata sulla scena
        ConfigureButtonsForScene();
        
        // Setup listeners
        SetupEventListeners();
        
        // Setup valori iniziali
        SetupInitialValues();
        
        // Aggiungi effetti sonori
        AddSoundEffectsToButtons();
        
        Debug.Log("Settings panel initialization complete");
    }
    
    private void ConfigureButtonsForScene()
    {
        bool isMainMenu = SceneManager.GetActiveScene().name.Equals("MainMenu");
        
        if (isMainMenu)
        {
            // MainMenu: mostra quit button
            if (quitPanelButton != null)
            {
                quitPanelButton.gameObject.SetActive(true);
                quitPanelButton.onClick.AddListener(OnQuitPanelButtonClicked);
            }
        }
        else
        {
            // GameScene: nascondi quit button
            if (quitPanelButton != null)
            {
                quitPanelButton.gameObject.SetActive(false);
            }
        }
    }
    
    private void SetupEventListeners()
    {
        // Rimuovi listener esistenti per evitare duplicati
        if (muteMainButton != null)
        {
            muteMainButton.onClick.RemoveAllListeners();
            muteMainButton.onClick.AddListener(OnMuteMainButtonClicked);
        }
        
        if (muteEffectsButton != null)
        {
            muteEffectsButton.onClick.RemoveAllListeners();
            muteEffectsButton.onClick.AddListener(OnMuteEffectsButtonClicked);
        }
        
        if (muteMusicButton != null)
        {
            muteMusicButton.onClick.RemoveAllListeners();
            muteMusicButton.onClick.AddListener(OnMuteMusicButtonClicked);
        }
      
        // Slider listeners
        if (sliderMainVolume != null)
        {
            sliderMainVolume.onValueChanged.RemoveAllListeners();
            sliderMainVolume.onValueChanged.AddListener(OnSliderMainVolumeChanged);
        }
        
        if (sliderEffectsVolume != null)
        {
            sliderEffectsVolume.onValueChanged.RemoveAllListeners();
            sliderEffectsVolume.onValueChanged.AddListener(OnSliderEffectsVolumeChanged);
        }
        
        if (sliderMusicVolume != null)
        {
            sliderMusicVolume.onValueChanged.RemoveAllListeners();
            sliderMusicVolume.onValueChanged.AddListener(OnSliderMusicVolumeChanged);
        }
    }
    
    private void SetupInitialValues()
    {
        // Setup valori iniziali slider
        if (sliderMainVolume != null)
        {
            sliderMainVolume.value = 1f;
            lastMainVolume = sliderMainVolume.value;
        }
        
        if (sliderEffectsVolume != null)
        {
            sliderEffectsVolume.value = 1f; 
            lastEffectsVolume = sliderEffectsVolume.value;
        }
        
        if (sliderMusicVolume != null)
        {
            sliderMusicVolume.value = 1f;
            lastMusicVolume = sliderMusicVolume.value;
        }
    }
    
    private void AddSoundEffectsToButtons()
    {
        bool isMainMenu = SceneManager.GetActiveScene().name.Equals("MainMenu");
        
        // Solo nel MainMenu aggiungiamo gli effetti sonori al quit button
        if (isMainMenu && quitPanelButton != null)
        {
            AddSoundEffectsToButton(quitPanelButton);
        }

        // Aggiungi effetti agli altri bottoni
        if (muteMainButton != null) AddSoundEffectsToButton(muteMainButton);
        if (muteEffectsButton != null) AddSoundEffectsToButton(muteEffectsButton);
        if (muteMusicButton != null) AddSoundEffectsToButton(muteMusicButton);
    }
    
    private void AddSoundEffectsToButton(Button button)
    {
        if (button == null) return;
        
        // Add hover sound
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        
        // Rimuovi trigger esistenti dello stesso tipo per evitare duplicati
        eventTrigger.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerEnter);
            
        // Setup pointer enter event (hover)
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((data) => {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonHover();
        });
        eventTrigger.triggers.Add(pointerEnterEntry);
        
        // Add click sound via additional listener
        button.onClick.AddListener(() => {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        });
    }
    
    private void OnSliderMusicVolumeChanged(float newVolume)
    {
        if (!isMusicMuted)
        {
            lastMusicVolume = newVolume;
            UpdateMusicVolume(newVolume);
        }
    }
    
    private void OnSliderEffectsVolumeChanged(float newVolume)
    {
        if (!isEffectsMuted)
        {
            lastEffectsVolume = newVolume;
            UpdateEffectsVolume(newVolume);
        }
    }
    
    private void OnSliderMainVolumeChanged(float newVolume)
    {
        if (!isMainMuted)
        {
            lastMainVolume = newVolume;
            UpdateMainVolume(newVolume);
        }
    }
    
    private void OnMuteMusicButtonClicked()
    {
        isMusicMuted = !isMusicMuted;
        
        if (isMusicMuted)
        {
            lastMusicVolume = sliderMusicVolume != null ? sliderMusicVolume.value : lastMusicVolume;
            UpdateMusicVolume(0f);
            if (musicVolumeImage != null) musicVolumeImage.sprite = mutedSprite;
        }
        else
        {
            if (sliderMusicVolume != null) sliderMusicVolume.value = lastMusicVolume;
            UpdateMusicVolume(lastMusicVolume);
            if (musicVolumeImage != null) musicVolumeImage.sprite = unmutedSprite;
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.Music, isMusicMuted);
    }
    
    private void OnMuteEffectsButtonClicked()
    {
        isEffectsMuted = !isEffectsMuted;
        
        if (isEffectsMuted)
        {
            lastEffectsVolume = sliderEffectsVolume != null ? sliderEffectsVolume.value : lastEffectsVolume;
            UpdateEffectsVolume(0f);
            if (effectsVolumeImage != null) effectsVolumeImage.sprite = mutedSprite;
        }
        else
        {
            if (sliderEffectsVolume != null) sliderEffectsVolume.value = lastEffectsVolume;
            UpdateEffectsVolume(lastEffectsVolume);
            if (effectsVolumeImage != null) effectsVolumeImage.sprite = unmutedSprite;
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.SFX, isEffectsMuted);
    }
    
    private void OnMuteMainButtonClicked()
    {
        isMainMuted = !isMainMuted;
        
        if (isMainMuted)
        {
            lastMainVolume = sliderMainVolume != null ? sliderMainVolume.value : lastMainVolume;
            UpdateMainVolume(0f);
            if (mainVolumeImage != null) mainVolumeImage.sprite = mutedSprite;
        }
        else
        {
            if (sliderMainVolume != null) sliderMainVolume.value = lastMainVolume;
            UpdateMainVolume(lastMainVolume);
            if (mainVolumeImage != null) mainVolumeImage.sprite = unmutedSprite;
        }
       
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.Master, isMainMuted);
    }
    
    private void OnQuitPanelButtonClicked()
    {
        // Questo metodo viene chiamato solo dal MainMenu
        if (optionButtonGameObject != null)
            optionButtonGameObject.SetActive(true);
            
        gameObject.SetActive(false);
    }
    
    private void UpdateMainVolume(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(volume);
        else
            AudioListener.volume = volume;
    }
    
    private void UpdateEffectsVolume(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(volume);
    }
    
    private void UpdateMusicVolume(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(volume);
    }
}