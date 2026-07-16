using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameStateService — State machine toàn game.
 * - Quản lý các state: Menu, Playing, Paused, GameOver, ...
 * - Validate transition hợp lệ
 * - Fire GameStateChangedEvent qua EventBus
 * - Fire C# event OnStateChanged cho hệ thống cần callback ngay
 */

public class GameStateService : IGameStateService
{
    #region Private Fields

    private GameState currentState = GameState.None;
    private GameState previousState = GameState.None;

    // Danh sách transition hợp lệ: (from → to)
    private static readonly HashSet<(GameState, GameState)> validTransitions = new()
    {
        // Boot
        (GameState.None,     GameState.Boot),
        (GameState.Boot,     GameState.MainMenu),
        (GameState.Boot,     GameState.Loading),

        // MainMenu
        (GameState.MainMenu, GameState.Loading),
        (GameState.MainMenu, GameState.Cutscene),

        // Loading → có thể về Menu hoặc vào Game
        (GameState.Loading,  GameState.MainMenu),
        (GameState.Loading,  GameState.Playing),
        (GameState.Loading,  GameState.Cutscene),

        // Playing
        (GameState.Playing,  GameState.Paused),
        (GameState.Playing,  GameState.GameOver),
        (GameState.Playing,  GameState.Victory),
        (GameState.Playing,  GameState.Cutscene),
        (GameState.Playing,  GameState.Loading),

        // Paused
        (GameState.Paused,   GameState.Playing),
        (GameState.Paused,   GameState.MainMenu),

        // GameOver / Victory → về Menu
        (GameState.GameOver, GameState.MainMenu),
        (GameState.GameOver, GameState.Loading),
        (GameState.Victory,  GameState.MainMenu),
        (GameState.Victory,  GameState.Loading),

        // Cutscene
        (GameState.Cutscene, GameState.Playing),
        (GameState.Cutscene, GameState.MainMenu),
    };

    #endregion

    #region Properties

    public GameState CurrentState  => currentState;
    public GameState PreviousState => previousState;

    #endregion

    #region Events

    public event Action<GameState, GameState> OnStateChanged;

    #endregion

    #region Transitions

    public bool ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.Log($"[GameStateService] Already in {newState}, ignored");
            return false;
        }

        if (!CanChangeTo(newState))
        {
            Debug.LogWarning($"[GameStateService] Invalid transition: {currentState} → {newState}");
            return false;
        }

        previousState = currentState;
        currentState  = newState;

        Debug.Log($"[GameStateService] {previousState} → {currentState}");

        // Fire C# event
        OnStateChanged?.Invoke(previousState, currentState);

        // Fire EventBus
        EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent
        {
            PreviousState = previousState,
            NewState      = currentState,
        });

        return true;
    }

    public bool CanChangeTo(GameState newState)
    {
        return validTransitions.Contains((currentState, newState));
    }

    #endregion

    #region Checks

    public bool IsState(GameState state) => currentState == state;

    public bool IsAnyState(params GameState[] states)
    {
        for (int i = 0; i < states.Length; i++)
            if (currentState == states[i]) return true;
        return false;
    }

    #endregion
}