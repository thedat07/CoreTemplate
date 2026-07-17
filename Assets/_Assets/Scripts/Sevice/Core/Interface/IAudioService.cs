using UnityEngine;

public interface IAudioService
{
    // --- Volume Properties ---
    float MasterVolume { get; set; }
    float SfxVolume { get; set; }
    float MusicVolume { get; set; }
    bool MasterMuted { get; set; }
    bool SfxMuted { get; set; }
    bool MusicMuted { get; set; }

    // --- SFX ---
    void PlaySFX(AudioClip clip, float volume = 1f);
    void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = 1f);
    void StopAllSFX();

    // --- Music ---
    void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true);
    void StopMusic();
    void PauseMusic();
    void ResumeMusic();

    // --- Settings ---
    void SetMasterVolume(float volume);
    void SaveVolumeSettings();
    void LoadVolumeSettings();
}
