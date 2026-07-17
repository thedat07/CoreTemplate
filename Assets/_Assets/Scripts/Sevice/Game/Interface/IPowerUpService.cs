using System;
using System.Collections.Generic;

/*
 * IPowerUpService — Interface cho hệ thống PowerUp.
 * Quản lý các chỉ số tăng cường: Attack, Survival, Bonus.
 *
 * Dữ liệu được load từ QuickSheet ScriptableObject:
 *   Resources.Load<PowerUps>("Data/PowerUps")
 *   Resources.Load<LevelPreview>("Data/LevelPreview")
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
    PowerUpsData GetData(PowerUpType type);

    // --- Cost ---
    int GetUpgradeCost(PowerUpType type);
    int GetUpgradeCostAtLevel(PowerUpType type, int level);

    // --- Operations ---
    bool TryUpgrade(PowerUpType type);
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
