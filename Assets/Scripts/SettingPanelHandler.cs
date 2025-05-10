using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    
    void Awake()
    {
        gameObject.SetActive(false);
        
      
        if (mutedSprite == null)
            mutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/muted.png");
            
        if (unmutedSprite == null)
            unmutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/unmuted.png");
    }
    
    void Start()
    {
       
        quitPanelButton.onClick.AddListener(OnQuitPanelButtonClicked);
        muteMainButton.onClick.AddListener(OnMuteMainButtonClicked);
        muteEffectsButton.onClick.AddListener(OnMuteEffectsButtonClicked);
        muteMusicButton.onClick.AddListener(OnMuteMusicButtonClicked);
        
      
        sliderMainVolume.onValueChanged.AddListener(OnSliderMainVolumeChanged);
        sliderEffectsVolume.onValueChanged.AddListener(OnSliderEffectsVolumeChanged);
        sliderMusicVolume.onValueChanged.AddListener(OnSliderMusicVolumeChanged);

       
        sliderMainVolume.value = 1f;
        sliderEffectsVolume.value = 1f; 
        sliderMusicVolume.value = 1f;
        
        lastMainVolume = sliderMainVolume.value;
        lastEffectsVolume = sliderEffectsVolume.value;
        lastMusicVolume = sliderMusicVolume.value;
        
      
        AddSoundEffectsToButton(quitPanelButton);
        AddSoundEffectsToButton(muteMainButton);
        AddSoundEffectsToButton(muteEffectsButton);
        AddSoundEffectsToButton(muteMusicButton);
    }
    
    private void AddSoundEffectsToButton(Button button)
    {
        // Add hover sound
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            
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
        Debug.Log($"Volume Musica cambiato a: {newVolume}");
    }
    
    private void OnSliderEffectsVolumeChanged(float newVolume)
    {
        if (!isEffectsMuted)
        {
            lastEffectsVolume = newVolume;
            UpdateEffectsVolume(newVolume);
        }
        Debug.Log($"Volume Effetti cambiato a: {newVolume}");
    }
    
    private void OnSliderMainVolumeChanged(float newVolume)
    {
        if (!isMainMuted)
        {
            lastMainVolume = newVolume;
            UpdateMainVolume(newVolume);
        }
        Debug.Log($"Volume Principale cambiato a: {newVolume}");
    }
    
    private void OnMuteMusicButtonClicked()
    {
        isMusicMuted = !isMusicMuted;
        
        if (isMusicMuted)
        {
            lastMusicVolume = sliderMusicVolume.value;
            UpdateMusicVolume(0f);
            musicVolumeImage.sprite = mutedSprite;
            Debug.Log("Musica mutata");
        }
        else
        {
            sliderMusicVolume.value = lastMusicVolume;
            UpdateMusicVolume(lastMusicVolume);
            musicVolumeImage.sprite = unmutedSprite;
            Debug.Log("Musica non più mutata");
        }
        
      
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.Music, isMusicMuted);
    }
    
    private void OnMuteEffectsButtonClicked()
    {
        isEffectsMuted = !isEffectsMuted;
        
        if (isEffectsMuted)
        {
            lastEffectsVolume = sliderEffectsVolume.value;
            UpdateEffectsVolume(0f);
            effectsVolumeImage.sprite = mutedSprite;
            Debug.Log("Effetti mutati");
        }
        else
        {
            sliderEffectsVolume.value = lastEffectsVolume;
            UpdateEffectsVolume(lastEffectsVolume);
            effectsVolumeImage.sprite = unmutedSprite;
            Debug.Log("Effetti non più mutati");
        }
        
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.SFX, isEffectsMuted);
    }
    
    private void OnMuteMainButtonClicked()
    {
        isMainMuted = !isMainMuted;
        
        if (isMainMuted)
        {
            lastMainVolume = sliderMainVolume.value;
            UpdateMainVolume(0f);
            mainVolumeImage.sprite = mutedSprite;
            Debug.Log("Volume principale mutato");
        }
        else
        {
            sliderMainVolume.value = lastMainVolume;
            UpdateMainVolume(lastMainVolume);
            mainVolumeImage.sprite = unmutedSprite;
            Debug.Log("Volume principale non più mutato");
        }
       
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuted(AudioManager.AudioSourceType.Master, isMainMuted);
    }
    
    private void OnQuitPanelButtonClicked()
    {
        Debug.Log("Quit Panel Button Clicked");
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