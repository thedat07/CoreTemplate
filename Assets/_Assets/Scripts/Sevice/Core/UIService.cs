using System;
using BlitzyUI;
using UnityEngine;

using UScreen = BlitzyUI.Screen;

/*
 * UIService — Wrapper quản lý UI qua BlitzyUI.
 * - Push / Pop screen theo stack
 * - Fire EventBus khi UI thay đổi
 */

public class UIService : IUIService
{
    #region Screen IDs

    public UScreen.Id MenuScreenId     { get; } = new UScreen.Id("Menu",     "MenuScreen");
    public UScreen.Id SettingsScreenId { get; } = new UScreen.Id("Settings", "SettingsScreen");
    public UScreen.Id GameplayScreenId { get; } = new UScreen.Id("Gameplay", "GameplayScreen");
    public UScreen.Id PopupScreenId    { get; } = new UScreen.Id("Popup",    "PopupScreen");

    #endregion

    #region Navigation

    public void OpenScreen(UScreen.Id id, UScreen.Data data = null, string prefabName = null, Action onPushed = null)
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("[UIService] UIManager.Instance is null");
            return;
        }

        UIManager.Instance.QueuePush(id, data ?? new UScreen.Data(), prefabName, (screen) =>
        {
            onPushed?.Invoke();

            EventBus<UIScreenPushedEvent>.Raise(new UIScreenPushedEvent
            {
                ScreenId = id.ToString()
            });
        });
    }

    public void CloseTopScreen(Action onPopped = null)
    {
        if (UIManager.Instance == null) return;

        UScreen top = UIManager.Instance.GetTopScreen();
        if (top == null) return;

        string topId = top.id.ToString();

        UIManager.Instance.QueuePop((poppedId) =>
        {
            onPopped?.Invoke();

            EventBus<UIScreenPoppedEvent>.Raise(new UIScreenPoppedEvent
            {
                ScreenId = topId
            });

            // Fire focus event for new top screen
            var newTop = UIManager.Instance.GetTopScreen();
            if (newTop != null)
            {
                EventBus<UIFocusChangedEvent>.Raise(new UIFocusChangedEvent
                {
                    FocusedScreenId = newTop.id.ToString()
                });
            }
        });
    }

    public void CloseToScreen(UScreen.Id id, bool include = false, Action onPopped = null)
    {
        if (UIManager.Instance == null) return;

        UIManager.Instance.QueuePopTo(id, include, (poppedId) =>
        {
            onPopped?.Invoke();

            EventBus<UIScreenPoppedEvent>.Raise(new UIScreenPoppedEvent
            {
                ScreenId = poppedId.ToString()
            });
        });
    }

    public void CloseAll()
    {
        if (UIManager.Instance == null) return;

        // Pop to a non-existent screen to clear everything
        var dummyId = new UScreen.Id("__NONEXISTENT__");
        UIManager.Instance.QueuePopTo(dummyId, false);
    }

    #endregion

    #region Query

    public UScreen GetTopScreen()
    {
        return UIManager.Instance?.GetTopScreen();
    }

    public T GetScreen<T>(UScreen.Id id) where T : UScreen
    {
        return UIManager.Instance?.GetScreen<T>(id);
    }

    public bool IsScreenInStack(UScreen.Id id)
    {
        return UIManager.Instance?.GetScreen(id) != null;
    }

    #endregion

    #region Visibility

    public void ShowUI()
    {
        if (UIManager.Instance == null) return;
        UIManager.Instance.SetVisibility(true);

        EventBus<UIVisibilityChangedEvent>.Raise(new UIVisibilityChangedEvent { Visible = true });
    }

    public void HideUI()
    {
        if (UIManager.Instance == null) return;
        UIManager.Instance.SetVisibility(false);

        EventBus<UIVisibilityChangedEvent>.Raise(new UIVisibilityChangedEvent { Visible = false });
    }

    public bool IsUIVisible => UIManager.Instance?.IsVisible() ?? true;

    #endregion
}