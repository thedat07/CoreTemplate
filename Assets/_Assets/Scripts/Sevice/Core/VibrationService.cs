using UnityServiceLocator;
using UnityEngine;

/*
 * VibrationService — Rung thiết bị di động (Android / iOS).
 * - Wrapper qua Handheld.Vibrate() và các pattern iOS/Android
 * - Dùng DataService để lưu trạng thái bật/tắt
 * - Fire EventBus khi rung hoặc thay đổi settings
 */

public class VibrationService : IVibrationService
{
    private const string VIBRATION_KEY = "VibrationEnabled";

    private readonly IDataService dataService;
    private bool isEnabled = true;

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled == value) return;
            isEnabled = value;
            SaveSettings();

            EventBus<VibrationSettingsChangedEvent>.Raise(new VibrationSettingsChangedEvent
            {
                Enabled = isEnabled
            });
        }
    }

    public VibrationService(IDataService dataService)
    {
        this.dataService = dataService;
        LoadSettings();
    }

    // ----- Public API -----

    public void Vibrate(VibrationPreset preset = VibrationPreset.Light)
    {
        if (!isEnabled) return;

        // Fire event cho hệ thống khác lắng nghe
        EventBus<VibrationEvent>.Raise(new VibrationEvent
        {
            Preset = preset,
            DurationMs = GetDurationForPreset(preset)
        });

        if (Application.isMobilePlatform)
        {
            Handheld.Vibrate();
        }
        else
        {
            Debug.Log($"[VibrationService] {preset} (no mobile hardware)");
        }
    }

    public void Vibrate(float durationMs)
    {
        if (!isEnabled) return;

        EventBus<VibrationEvent>.Raise(new VibrationEvent
        {
            Preset = VibrationPreset.Light,
            DurationMs = durationMs
        });

        if (Application.isMobilePlatform)
        {
            Handheld.Vibrate();
        }
    }

    public void VibratePop()
    {
        // iOS: UIImpactFeedbackStyle.Light — dùng Handheld.Vibrate() làm fallback
        if (!isEnabled) return;
        Handheld.Vibrate();
    }

    public void VibratePeek()
    {
        // iOS: UIImpactFeedbackStyle.Medium
        if (!isEnabled) return;
        Handheld.Vibrate();
    }

    public void VibrateNope()
    {
        // iOS: UINotificationFeedbackStyle.Error
        if (!isEnabled) return;
        Handheld.Vibrate();
    }

    // ----- Settings -----

    public void SaveSettings()
    {
        dataService.Save(VIBRATION_KEY, isEnabled);
    }

    public void LoadSettings()
    {
        isEnabled = dataService.Load(VIBRATION_KEY, true);
    }

    // ----- Helpers -----

    private float GetDurationForPreset(VibrationPreset preset)
    {
        return preset switch
        {
            VibrationPreset.Light       => 50f,
            VibrationPreset.Medium      => 100f,
            VibrationPreset.Heavy       => 200f,
            VibrationPreset.ButtonTap   => 30f,
            VibrationPreset.Notification=> 150f,
            VibrationPreset.Error       => 300f,
            VibrationPreset.Success     => 100f,
            _                           => 100f,
        };
    }
}