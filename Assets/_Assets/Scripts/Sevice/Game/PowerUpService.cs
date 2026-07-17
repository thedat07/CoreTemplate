using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * PowerUpService — Quản lý tất cả PowerUp trong game.
 *
 * ⚔️ Attack:
 *   💪 Stronger     → Gây nhiều sát thương hơn
 *   ⚡ ShootFaster  → Bắn nhanh hơn
 *   ➕ MoreBullets  → Bắn thêm đạn
 *   🎯 LongerRange  → Bắn xa hơn
 *   💥 BiggerBoom   → Vụ nổ lớn hơn
 *
 * ❤️ Survival:
 *   ❤️ MoreHearts   → Nhiều máu hơn
 *   🩹 Heal         → Hồi máu (tức thời)
 *   🛡 Shield       → Chặn bớt sát thương
 *   👟 RunFaster    → Di chuyển nhanh hơn
 *
 * ⭐ Bonus:
 *   🪙 MoreCoins     → Nhận nhiều xu hơn
 *   🧲 BiggerMagnet  → Hút xu từ xa hơn
 *   🎁 LuckyBox      → Dễ nhận rương thưởng hơn
 *   ⭐ Lucky         → May mắn hơn (tăng cơ hội nhận thưởng hiếm)
 *
 * ---
 * Dữ liệu được load từ QuickSheet ScriptableObject:
 *   Resources.Load<PowerUps>("Data/PowerUps")
 *   Resources.Load<LevelPreview>("Data/LevelPreview")
 *
 * Cost formula: Cost = ROUND(BaseCost × Growth^(Level-1), 0)
 * Stat formula: Stat = BaseValue + PerLevel × Level
 */

public class PowerUpService : IPowerUpService
{
    #region Constants

    private const string POWERUPS_PATH = "Data/PowerUps";
    private const string LEVEL_PREVIEW_PATH = "Data/LevelPreview";

    #endregion

    #region Private Fields

    private readonly Dictionary<PowerUpType, int> levels = new();
    private readonly Dictionary<PowerUpCategory, List<PowerUpType>> groupedByCategory = new();
    private Dictionary<PowerUpType, PowerUpsData> upgradesLookup;
    private PowerUps powerUps;
    private LevelPreview levelPreview;

    #endregion

    #region Events

    public event Action<PowerUpType, int, int> OnPowerUpLevelChanged;
    public event Action<PowerUpType> OnPowerUpActivated;

    #endregion

    #region Properties

    public IReadOnlyDictionary<PowerUpType, int> AllLevels => levels;

    #endregion

    #region Initialization

    public PowerUpService()
    {
        // Load QuickSheet PowerUps
        powerUps = Resources.Load<PowerUps>(POWERUPS_PATH);
        if (powerUps == null || powerUps.dataArray == null)
        {
            Debug.LogError($"[PowerUpService] Kh\xf4ng t\xecm th\x1ea5y PowerUps config t\u1ea1i Resources/{POWERUPS_PATH}");
            return;
        }

        // Load QuickSheet LevelPreview
        levelPreview = Resources.Load<LevelPreview>(LEVEL_PREVIEW_PATH);
        if (levelPreview == null || levelPreview.dataArray == null)
        {
            Debug.LogWarning($"[PowerUpService] Kh\xf4ng t\xecm th\x1ea5y LevelPreview config t\u1ea1i Resources/{LEVEL_PREVIEW_PATH}");
        }

        // Build lookup: PowerUpType → PowerUpsData
        upgradesLookup = new Dictionary<PowerUpType, PowerUpsData>();
        foreach (var data in powerUps.dataArray)
        {
            upgradesLookup[data.POWERUPTYPE] = data;
        }

        // Khởi tạo level mặc định = 0
        foreach (PowerUpType type in Enum.GetValues(typeof(PowerUpType)))
        {
            levels[type] = 0;
        }

        // Nhóm theo category
        foreach (var kvp in upgradesLookup)
        {
            var cat = kvp.Value.POWERUPCATEGORY;
            if (!groupedByCategory.ContainsKey(cat))
                groupedByCategory[cat] = new List<PowerUpType>();
            if (!groupedByCategory[cat].Contains(kvp.Key))
                groupedByCategory[cat].Add(kvp.Key);
        }
    }

    #endregion

    #region Public API — Queries

    public int GetLevel(PowerUpType type)
    {
        return levels.TryGetValue(type, out int level) ? level : 0;
    }

    public int GetMaxLevel(PowerUpType type)
    {
        return TryGetData(type, out var data) ? data.Maxlevel : 0;
    }

    public PowerUpsData GetData(PowerUpType type)
    {
        if (TryGetData(type, out var data))
            return data;
        // Trả về fallback an toàn để tránh NullReferenceException
        return new PowerUpsData
        {
            POWERUPCATEGORY = PowerUpCategory.Attack,
            POWERUPTYPE = type,
            Description = "(data not loaded)"
        };
    }

    /// <summary>Stat = BaseValue + PerLevel × Level</summary>
    public float GetEffectValue(PowerUpType type)
    {
        if (!TryGetData(type, out var data)) return 0f;
        return data.Basevalue + data.Perlevel * GetLevel(type);
    }

    public bool IsMaxLevel(PowerUpType type)
    {
        return GetLevel(type) >= GetMaxLevel(type);
    }

    public bool IsUnlocked(PowerUpType type)
    {
        return GetLevel(type) > 0;
    }

    #endregion

    #region Public API — Cost

    /// <summary>Chi phí nâng từ level hiện tại lên cấp tiếp theo.</summary>
    public int GetUpgradeCost(PowerUpType type)
    {
        return GetUpgradeCostAtLevel(type, GetLevel(type));
    }

    /// <summary>Cost = ROUND(BaseCost × Growth^(Level-1), 0)</summary>
    public int GetUpgradeCostAtLevel(PowerUpType type, int level)
    {
        if (!TryGetData(type, out var data)) return 0;
        if (level >= data.Maxlevel) return 0;

        float raw = data.Basecost * Mathf.Pow(data.Costgrowth, level);
        return Mathf.RoundToInt(raw);
    }

    #endregion

    #region Public API — Operations

    public bool TryUpgrade(PowerUpType type)
    {
        if (IsMaxLevel(type))
        {
            Debug.Log($"[PowerUpService] {type} already at max level ({GetMaxLevel(type)})");
            return false;
        }

        int oldLevel = GetLevel(type);
        SetLevel(type, oldLevel + 1);
        return true;
    }

    public void SetLevel(PowerUpType type, int level)
    {
        int maxLevel = GetMaxLevel(type);
        int clampedLevel = Mathf.Clamp(level, 0, maxLevel);

        int oldLevel = GetLevel(type);
        if (oldLevel == clampedLevel) return;

        levels[type] = clampedLevel;

        Debug.Log($"[PowerUpService] {type}: {oldLevel} \u2192 {clampedLevel}");

        OnPowerUpLevelChanged?.Invoke(type, oldLevel, clampedLevel);
        EventBus<PowerUpLevelChangedEvent>.Raise(new PowerUpLevelChangedEvent
        {
            Type = type,
            PreviousLevel = oldLevel,
            NewLevel = clampedLevel,
        });
    }

    public void ResetAll()
    {
        foreach (PowerUpType type in Enum.GetValues(typeof(PowerUpType)))
        {
            SetLevel(type, 0);
        }
        Debug.Log("[PowerUpService] All power-ups reset to level 0");
    }

    public void Activate(PowerUpType type)
    {
        Debug.Log($"[PowerUpService] Activated {type}");

        OnPowerUpActivated?.Invoke(type);
        EventBus<PowerUpActivatedEvent>.Raise(new PowerUpActivatedEvent
        {
            Type = type,
        });
    }

    #endregion

    #region Category Queries

    public IReadOnlyList<PowerUpType> GetPowerUpsInCategory(PowerUpCategory category)
    {
        if (groupedByCategory.TryGetValue(category, out var list))
            return list.AsReadOnly();
        return Array.Empty<PowerUpType>();
    }

    #endregion

    #region Helper Methods

    private bool TryGetData(PowerUpType type, out PowerUpsData data)
    {
        if (upgradesLookup != null && upgradesLookup.TryGetValue(type, out data))
            return true;
        data = default;
        return false;
    }

    #endregion
}
