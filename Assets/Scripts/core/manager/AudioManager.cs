using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHoverSound;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip meleeSound;
    [SerializeField] private AudioClip rockImpact;
    [SerializeField] private AudioClip rangedSound;
    [SerializeField] private AudioClip melee2Sound;
    [SerializeField] private AudioClip looseSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip hitSound; 
    [SerializeField] private AudioClip hit2Sound; 

    // Lambda one-liners
    public void PlayMeleeSound() => PlaySFX(meleeSound);
    public void PlayRangedSound() => PlaySFX(rangedSound);
    public void PlayMelee2Sound() => PlaySFX(melee2Sound);
    public void PlayLooseSound() => PlaySFX(looseSound);
    public void PlayWinSound() => PlaySFX(winSound);
    public void PlayLevelUpSound() => PlaySFX(levelUpSound);
    public void PlayHitSound() => PlaySFX(hitSound);
    public void PlayHit2Sound() => PlaySFX(hit2Sound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayButtonHover() => PlaySFX(buttonHoverSound);
    public void PlayRockImpact() => PlaySFX(rockImpact);

    private const string MASTER_VOLUME = "MasterVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_VOLUME = "MusicVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (menuMusic != null && musicSource != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if(scene.name != "GameScene" && scene.name != "GameScene_second"){
        musicSource.Stop();
      } 
      else{
        if (menuMusic != null && musicSource != null)
        {
            if(scene.name == "GameScene_second")
                musicSource.pitch = 0.8f;
                else
                musicSource.pitch = 1f;
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
      }

    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMasterVolume(float volume)
    {
        SetVolumeParameter(MASTER_VOLUME, volume);
        if(sfxSource != null && musicSource != null)
        {
            sfxSource.volume = volume;
            musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        SetVolumeParameter(SFX_VOLUME, volume);
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        SetVolumeParameter(MUSIC_VOLUME, volume);
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    private void SetVolumeParameter(string parameterName, float normalizedVolume) =>
        audioMixer?.SetFloat(parameterName, normalizedVolume > 0.001f ? 20f * Mathf.Log10(normalizedVolume) : -80f);

    public void SetMuted(AudioSourceType sourceType, bool isMuted)
    {
        switch(sourceType)
        {
            case AudioSourceType.Master:
                AudioListener.volume = isMuted ? 0f : 1f;
                break;
            case AudioSourceType.Music:
                if (musicSource != null)
                    musicSource.mute = isMuted;
                break;
            case AudioSourceType.SFX:
                if (sfxSource != null)
                    sfxSource.mute = isMuted;
                break;
        }
    }

    public enum AudioSourceType
    {
        Master,
        SFX,
        Music
    }
}
