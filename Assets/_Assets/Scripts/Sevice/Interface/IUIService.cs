using System;
using UScreen = BlitzyUI.Screen;

public interface IUIService
{
    // --- Screen IDs ---
    UScreen.Id MenuScreenId { get; }
    UScreen.Id SettingsScreenId { get; }
    UScreen.Id GameplayScreenId { get; }
    UScreen.Id PopupScreenId { get; }
    UScreen.Id LoadingScreenId { get; }

    // --- Navigation ---
    void OpenScreen(UScreen.Id id, UScreen.Data data = null, string prefabName = null, Action onPushed = null);
    void CloseTopScreen(Action onPopped = null);
    void CloseToScreen(UScreen.Id id, bool include = false, Action onPopped = null);
    void CloseAll();

    // --- Loading Screen ---
    void ShowLoading(string message = null, bool showProgress = false);
    void UpdateLoadingProgress(float progress);
    void UpdateLoadingMessage(string message);
    void HideLoading(Action onComplete = null);
    bool IsLoading { get; }

    // --- Query ---
    UScreen GetTopScreen();
    T GetScreen<T>(UScreen.Id id) where T : UScreen;
    bool IsScreenInStack(UScreen.Id id);

    // --- Visibility ---
    void ShowUI();
    void HideUI();
    bool IsUIVisible { get; }
}