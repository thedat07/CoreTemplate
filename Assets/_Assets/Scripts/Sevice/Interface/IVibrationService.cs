public interface IVibrationService
{
    bool IsEnabled { get; set; }

    void Vibrate(VibrationPreset preset = VibrationPreset.Light);
    void Vibrate(float durationMs);
    void VibratePop();
    void VibratePeek();
    void VibrateNope();

    void SaveSettings();
    void LoadSettings();
}