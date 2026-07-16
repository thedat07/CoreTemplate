using System.Collections.Generic;
using UnityServiceLocator;
using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService
{
    #region Inspector

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private int sfxPoolSize = 5;

    #endregion

    #region Private Fields

    private const string DATA_KEY_PREFIX = "Audio_";

    private IDataService dataService;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    private bool masterMuted;
    private bool sfxMuted;
    private bool musicMuted;

    private List<AudioSource> sfxPool;
    private AudioSource musicSourceInstance;

    #endregion

    #region Properties

    public float MasterVolume
    {
        get => masterVolume;
        set { masterVolume = Mathf.Clamp01(value); ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set { sfxVolume = Mathf.Clamp01(value); ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    public float MusicVolume
    {
        get => musicVolume;
        set { musicVolume = Mathf.Clamp01(value); ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    public bool MasterMuted
    {
        get => masterMuted;
        set { masterMuted = value; ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    public bool SfxMuted
    {
        get => sfxMuted;
        set { sfxMuted = value; ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    public bool MusicMuted
    {
        get => musicMuted;
        set { musicMuted = value; ApplyVolume(); RaiseAudioSettingsEvent(); }
    }

    #endregion

    #region Initialization

    private bool isInitialized;

    public void Initialize(IDataService dataService = null)
    {
        if (isInitialized) return;
        isInitialized = true;

        // Music source
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSourceInstance = musicSource;

        // SFX pool
        sfxPool = new List<AudioSource>(sfxPoolSize);
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            sfxPool.Add(source);
        }

        this.dataService = dataService ?? ServiceLocator.Global.Get<IDataService>();
        LoadVolumeSettings();
        ApplyVolume();

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region SFX

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSFXSource();
        if (source == null) return;

        float finalVolume = CalculateSfxVolume(volume);
        source.volume = finalVolume;
        source.PlayOneShot(clip, finalVolume);
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, CalculateSfxVolume(volume));
    }

    public void StopAllSFX()
    {
        for (int i = 0; i < sfxPool.Count; i++)
            sfxPool[i]?.Stop();
    }

    private AudioSource GetAvailableSFXSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
            if (!sfxPool[i].isPlaying)
                return sfxPool[i];

        // Expand pool
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.spatialBlend = 0f;
        sfxPool.Add(newSource);
        return newSource;
    }

    #endregion

    #region Music

    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;

        musicSourceInstance.Stop();
        musicSourceInstance.clip = clip;
        musicSourceInstance.loop = loop;
        musicSourceInstance.volume = CalculateMusicVolume(volume);
        musicSourceInstance.Play();
    }

    public void StopMusic() => musicSourceInstance.Stop();

    public void PauseMusic() => musicSourceInstance.Pause();

    public void ResumeMusic()
    {
        if (!musicSourceInstance.isPlaying)
            musicSourceInstance.UnPause();
    }

    #endregion

    #region Volume

    public void SetMasterVolume(float volume) => MasterVolume = volume;

    private float CalculateSfxVolume(float modifier)
    {
        if (masterMuted || sfxMuted) return 0f;
        return masterVolume * sfxVolume * Mathf.Clamp01(modifier);
    }

    private float CalculateMusicVolume(float modifier)
    {
        if (masterMuted || musicMuted) return 0f;
        return masterVolume * musicVolume * Mathf.Clamp01(modifier);
    }

    private void ApplyVolume()
    {
        for (int i = 0; i < sfxPool.Count; i++)
            if (sfxPool[i] != null)
                sfxPool[i].volume = CalculateSfxVolume(1f);

        if (musicSourceInstance != null)
            musicSourceInstance.volume = CalculateMusicVolume(1f);
    }

    private void RaiseAudioSettingsEvent()
    {
        EventBus<AudioSettingsChangedEvent>.Raise(new AudioSettingsChangedEvent
        {
            MasterVolume = masterVolume,
            SfxVolume = sfxVolume,
            MusicVolume = musicVolume,
            MasterMuted = masterMuted,
            SfxMuted = sfxMuted,
            MusicMuted = musicMuted,
        });
    }

    #endregion

    #region Persistence

    public void SaveVolumeSettings()
    {
        if (dataService == null) return;
        dataService.Save(DATA_KEY_PREFIX + "MasterVolume", masterVolume);
        dataService.Save(DATA_KEY_PREFIX + "SfxVolume", sfxVolume);
        dataService.Save(DATA_KEY_PREFIX + "MusicVolume", musicVolume);
        dataService.Save(DATA_KEY_PREFIX + "MasterMuted", masterMuted);
        dataService.Save(DATA_KEY_PREFIX + "SfxMuted", sfxMuted);
        dataService.Save(DATA_KEY_PREFIX + "MusicMuted", musicMuted);
    }

    public void LoadVolumeSettings()
    {
        if (dataService == null) return;
        masterVolume = dataService.Load(DATA_KEY_PREFIX + "MasterVolume", 1f);
        sfxVolume = dataService.Load(DATA_KEY_PREFIX + "SfxVolume", 1f);
        musicVolume = dataService.Load(DATA_KEY_PREFIX + "MusicVolume", 1f);
        masterMuted = dataService.Load(DATA_KEY_PREFIX + "MasterMuted", false);
        sfxMuted = dataService.Load(DATA_KEY_PREFIX + "SfxMuted", false);
        musicMuted = dataService.Load(DATA_KEY_PREFIX + "MusicMuted", false);
    }

    #endregion
}