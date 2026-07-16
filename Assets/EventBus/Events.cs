using UnityEngine.SceneManagement;

public interface IEvent { }

// ============================================
// Game State Events — dùng cho AudioService & các hệ thống khác
// ============================================

/// <summary>Fired when the game state changes (menu, playing, paused, game over...)</summary>
public struct GameStateChangedEvent : IEvent
{
    public GameState PreviousState { get; set; }
    public GameState NewState { get; set; }
}

public enum GameState
{
    None,
    Boot,
    MainMenu,
    Loading,
    Playing,
    Paused,
    GameOver,
    Victory,
    Cutscene
}

// ============================================
// Audio-specific Events
// ============================================

/// <summary>Fired when volume or mute settings change (for UI to respond)</summary>
public struct AudioSettingsChangedEvent : IEvent
{
    public float MasterVolume;
    public float SfxVolume;
    public float MusicVolume;
    public bool MasterMuted;
    public bool SfxMuted;
    public bool MusicMuted;
}

/// <summary>Fired when a specific audio action is requested (e.g. UI hover, click)</summary>
public struct AudioOneShotEvent : IEvent
{
    public AudioAction Action { get; set; }
}

public enum AudioAction
{
    ButtonHover,
    ButtonClick,
    OpenPanel,
    ClosePanel,
    Notification,
    Error,
    LevelComplete,
    Achievement
}

// ============================================
// Data Service Events
// ============================================

/// <summary>Fired after data is successfully saved</summary>
public struct DataSavedEvent : IEvent
{
    public string Key { get; set; }
    public string DataType { get; set; }
}

/// <summary>Fired after data is successfully loaded</summary>
public struct DataLoadedEvent : IEvent
{
    public string Key { get; set; }
    public string DataType { get; set; }
}

/// <summary>Fired after data is deleted</summary>
public struct DataDeletedEvent : IEvent
{
    public string Key { get; set; }
}

/// <summary>Fired when a data operation fails</summary>
public struct DataErrorEvent : IEvent
{
    public string Key { get; set; }
    public string Operation { get; set; }  // Save / Load / Delete
    public string ErrorMessage { get; set; }
}

// ============================================
// Vibration Events
// ============================================

/// <summary>Fired when a vibration action is requested</summary>
public struct VibrationEvent : IEvent
{
    public VibrationPreset Preset { get; set; }
    public float DurationMs { get; set; }
}

public enum VibrationPreset
{
    Light,
    Medium,
    Heavy,
    ButtonTap,
    Notification,
    Error,
    Success
}

/// <summary>Fired when vibration settings change</summary>
public struct VibrationSettingsChangedEvent : IEvent
{
    public bool Enabled { get; set; }
}

// ============================================
// UI Events
// ============================================

/// <summary>Fired when a screen is pushed (opened)</summary>
public struct UIScreenPushedEvent : IEvent
{
    public string ScreenId { get; set; }
}

/// <summary>Fired when a screen is popped (closed)</summary>
public struct UIScreenPoppedEvent : IEvent
{
    public string ScreenId { get; set; }
}

/// <summary>Fired when the top-most screen changes focus</summary>
public struct UIFocusChangedEvent : IEvent
{
    public string FocusedScreenId { get; set; }
}

/// <summary>Fired when the entire UI visibility toggles</summary>
public struct UIVisibilityChangedEvent : IEvent
{
    public bool Visible { get; set; }
}

// ============================================
// Scene Events
// ============================================

/// <summary>Fired when a scene starts loading</summary>
public struct SceneLoadingEvent : IEvent
{
    public string SceneName { get; set; }
    public LoadSceneMode Mode { get; set; }
}

/// <summary>Fired when a scene has finished loading</summary>
public struct SceneLoadedEvent : IEvent
{
    public string SceneName { get; set; }
    public LoadSceneMode Mode { get; set; }
}

/// <summary>Fired when a scene is unloaded</summary>
public struct SceneUnloadedEvent : IEvent
{
    public string SceneName { get; set; }
}

/// <summary>Fired when loading progress updates (async only)</summary>
public struct SceneProgressEvent : IEvent
{
    public string SceneName { get; set; }
    public float Progress { get; set; }
}

/// <summary>Fired when the loading screen appears / updates / hides</summary>
public struct UILoadingEvent : IEvent
{
    public LoadingState State { get; set; }
    public string Message { get; set; }
    public float Progress { get; set; }
}

public enum LoadingState
{
    Show,
    Update,
    Hide
}

// ============================================
// Analytics Events — giống Firebase Analytics
// ============================================

/// <summary>Fired when an analytics event is logged (TriggerService)</summary>
public struct AnalyticsEvent : IEvent
{
    public string EventName { get; set; }
    public System.Collections.Generic.IDictionary<string, object> Parameters { get; set; }
}

// ============================================
// Legacy samples (giữ lại để tham khảo)
// ============================================