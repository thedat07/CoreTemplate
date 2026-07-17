using System;
using System.Collections.Generic;

/*
 * IPowerUpService — Interface cho hệ thống PowerUp.
 * Quản lý các chỉ số tăng cường: Attack, Survival, Bonus.
 */

public interface IPowerUpService
{
    // --- Properties ---
    IReadOnlyDictionary<PowerUpType, int> AllLevels { get; }

    // --- Events ---
    event Action<PowerUpType, int, int> OnPowerUpLevelChanged; // (type, oldLevel, newLevel)
    event Action<PowerUpType> OnPowerUpActivated;

    // --- Queries ---
    int GetLevel(PowerUpType type);
    int GetMaxLevel(PowerUpType type);
    float GetEffectValue(PowerUpType type);
    bool IsMaxLevel(PowerUpType type);
    bool IsUnlocked(PowerUpType type);

    // --- Operations ---
    void Upgrade(PowerUpType type);
    void SetLevel(PowerUpType type, int level);
    void ResetAll();
    void Activate(PowerUpType type);

    // --- Category Queries ---
    IReadOnlyList<PowerUpType> GetPowerUpsInCategory(PowerUpCategory category);
}

public enum PowerUpCategory
{
    Attack,   // ⚔️
    Survival, // ❤️
    Bonus     // ⭐
}
