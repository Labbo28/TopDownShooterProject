using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelHandler : MonoBehaviour {
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


    
    private bool isMainMuted = false;
    private bool isEffectsMuted = false;
    private bool isMusicMuted = false;
    
    private float lastMainVolume = 1f;
    private float lastEffectsVolume = 1f;
    private float lastMusicVolume = 1f;

    private Sprite mutedSprite;
    private Sprite unmutedSprite;
    
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

        mutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/muted.png");
        unmutedSprite = Resources.Load<Sprite>("Assets/static_assets/Undead Survivor/Sprites/unmuted.png");
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
    }
    
    private void OnQuitPanelButtonClicked()
    {
        Debug.Log("Quit Panel Button Clicked");
        optionButtonGameObject.SetActive(true);
        gameObject.SetActive(false);
        
    }
    
    private void UpdateMainVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    private void UpdateEffectsVolume(float volume)
    {
        
    }
    
    private void UpdateMusicVolume(float volume)
    {
        
    }
    
    void Awake()
    {
        gameObject.SetActive(false);
    }
}