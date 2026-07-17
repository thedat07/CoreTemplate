using System;

public interface IGameStateService
{
    // --- State ---
    GameState CurrentState { get; }
    GameState PreviousState { get; }

    // --- Events ---
    event Action<GameState, GameState> OnStateChanged;  // (prev, new)

    // --- Transitions ---
    bool ChangeState(GameState newState);
    bool CanChangeTo(GameState newState);

    // --- Checks ---
    bool IsState(GameState state);
    bool IsAnyState(params GameState[] states);
}