using System;
using UScreen = BlitzyUI.Screen;

public interface IUIService
{
    // --- Screen IDs ---
    UScreen.Id MenuScreenId { get; }
    UScreen.Id SettingsScreenId { get; }
    UScreen.Id GameplayScreenId { get; }
    UScreen.Id PopupScreenId { get; }
    // --- Navigation ---
    void OpenScreen(UScreen.Id id, UScreen.Data data = null, string prefabName = null, Action onPushed = null);
    void CloseTopScreen(Action onPopped = null);
    void CloseToScreen(UScreen.Id id, bool include = false, Action onPopped = null);
    void CloseAll();

    // --- Query ---
    UScreen GetTopScreen();
    T GetScreen<T>(UScreen.Id id) where T : UScreen;
    bool IsScreenInStack(UScreen.Id id);

    // --- Visibility ---
    void ShowUI();
    void HideUI();
    bool IsUIVisible { get; }
}