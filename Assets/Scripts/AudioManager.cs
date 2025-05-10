using UnityEngine;
using UnityEngine.Audio;


/*
Bisogna creare un AudioMixer
al monento il volume viene gestito modificando il volume del AudioSource    
*/
public class AudioManager : MonoBehaviour
{
    // Singleton pattern
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
    
    // Volume parameters
    private const string MASTER_VOLUME = "MasterVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    
    private void Awake()
    {
        // Singleton pattern implementation
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
        // Start menu music if available
        if (menuMusic != null && musicSource != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSound);
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
        
        // Update SFX source volume directly for immediate feedback
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
    
    private void SetVolumeParameter(string parameterName, float normalizedVolume)
    {
        if (audioMixer != null)
        {
            // Convert normalized volume (0-1) to logarithmic scale for better audio perception
            // -80dB is near silence, 0dB is full volume
            float dbVolume = normalizedVolume > 0.001f ? 20f * Mathf.Log10(normalizedVolume) : -80f;
            audioMixer.SetFloat(parameterName, dbVolume);
        }
    }
    
 
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
    
    // Enum to identify audio source types
    public enum AudioSourceType
    {
        Master,
        SFX,
        Music
    }
}