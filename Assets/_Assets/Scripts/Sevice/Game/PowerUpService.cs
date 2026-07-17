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
 */

public class PowerUpService : IPowerUpService
{
    #region Constants

    private const int DEFAULT_MAX_LEVEL = 5;

    #endregion

    #region Static Data

    /// <summary>Định nghĩa cấu hình cho từng PowerUp.</summary>
    private static readonly Dictionary<PowerUpType, PowerUpDefinition> Definitions = new()
    {
        // ===== ⚔️ ATTACK =====
        [PowerUpType.Stronger] = new()
        {
            DisplayName = "Stronger",
            Icon = "\U0001f4aa",
            Description = "Gây nhiều sát thương hơn",
            Category = PowerUpCategory.Attack,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.25f,  // +25% damage per level
        },
        [PowerUpType.ShootFaster] = new()
        {
            DisplayName = "Shoot Faster",
            Icon = "\u26a1",
            Description = "Bắn nhanh hơn",
            Category = PowerUpCategory.Attack,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.15f,  // -15% cooldown per level
        },
        [PowerUpType.MoreBullets] = new()
        {
            DisplayName = "More Bullets",
            Icon = "\u2795",
            Description = "Bắn thêm đạn",
            Category = PowerUpCategory.Attack,
            MaxLevel = 3,
            BaseValue = 1,
            Increment = 1,      // +1 bullet per level
        },
        [PowerUpType.LongerRange] = new()
        {
            DisplayName = "Longer Range",
            Icon = "\U0001f3af",
            Description = "Bắn xa hơn",
            Category = PowerUpCategory.Attack,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.2f,   // +20% range per level
        },
        [PowerUpType.BiggerBoom] = new()
        {
            DisplayName = "Bigger Boom",
            Icon = "\U0001f4a5",
            Description = "Vụ nổ lớn hơn",
            Category = PowerUpCategory.Attack,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.3f,   // +30% explosion radius per level
        },

        // ===== ❤️ SURVIVAL =====
        [PowerUpType.MoreHearts] = new()
        {
            DisplayName = "More Hearts",
            Icon = "\u2764\ufe0f",
            Description = "Nhiều máu hơn",
            Category = PowerUpCategory.Survival,
            MaxLevel = 5,
            BaseValue = 3,
            Increment = 1,      // +1 max heart per level
        },
        [PowerUpType.Heal] = new()
        {
            DisplayName = "Heal",
            Icon = "\U0001fa79",
            Description = "Hồi máu",
            Category = PowerUpCategory.Survival,
            MaxLevel = 1,       // Instant, no level
            BaseValue = 1f,
            Increment = 0f,
        },
        [PowerUpType.Shield] = new()
        {
            DisplayName = "Shield",
            Icon = "\U0001f6e1",
            Description = "Chặn bớt sát thương",
            Category = PowerUpCategory.Survival,
            MaxLevel = 5,
            BaseValue = 0f,
            Increment = 0.15f,  // +15% damage blocked per level
        },
        [PowerUpType.RunFaster] = new()
        {
            DisplayName = "Run Faster",
            Icon = "\U0001f45f",
            Description = "Di chuyển nhanh hơn",
            Category = PowerUpCategory.Survival,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.12f,  // +12% speed per level
        },

        // ===== ⭐ BONUS =====
        [PowerUpType.MoreCoins] = new()
        {
            DisplayName = "More Coins",
            Icon = "\U0001fa99",
            Description = "Nhận nhiều xu hơn",
            Category = PowerUpCategory.Bonus,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.5f,   // +50% coins per level
        },
        [PowerUpType.BiggerMagnet] = new()
        {
            DisplayName = "Bigger Magnet",
            Icon = "\U0001f9f2",
            Description = "Hút xu từ xa hơn",
            Category = PowerUpCategory.Bonus,
            MaxLevel = 5,
            BaseValue = 1f,
            Increment = 0.3f,   // +30% magnet radius per level
        },
        [PowerUpType.LuckyBox] = new()
        {
            DisplayName = "Lucky Box",
            Icon = "\U0001f381",
            Description = "Dễ nhận rương thưởng hơn",
            Category = PowerUpCategory.Bonus,
            MaxLevel = 5,
            BaseValue = 0f,
            Increment = 0.2f,   // +20% box drop rate per level
        },
        [PowerUpType.Lucky] = new()
        {
            DisplayName = "Lucky",
            Icon = "\u2b50",
            Description = "May mắn hơn (tăng cơ hội nhận thưởng hiếm)",
            Category = PowerUpCategory.Bonus,
            MaxLevel = 5,
            BaseValue = 0f,
            Increment = 0.15f,  // +15% rare reward chance per level
        },
    };

    #endregion

    #region Private Fields

    private readonly Dictionary<PowerUpType, int> levels = new();
    private readonly Dictionary<PowerUpCategory, List<PowerUpType>> groupedByCategory = new();

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
        // Khởi tạo level mặc định = 0
        foreach (PowerUpType type in Enum.GetValues(typeof(PowerUpType)))
        {
            levels[type] = 0;
        }

        // Nhóm theo category
        foreach (var kvp in Definitions)
        {
            var cat = kvp.Value.Category;
            if (!groupedByCategory.ContainsKey(cat))
                groupedByCategory[cat] = new List<PowerUpType>();
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
        return Definitions.TryGetValue(type, out var def) ? def.MaxLevel : DEFAULT_MAX_LEVEL;
    }

    /// <summary>
    /// Trả về giá trị hiệu quả dạng số (multiplier / cộng dồn) dựa trên level hiện tại.
    /// Công thức: BaseValue + (Increment * Level)
    /// </summary>
    public float GetEffectValue(PowerUpType type)
    {
        if (!Definitions.TryGetValue(type, out var def))
            return 0f;

        return def.BaseValue + def.Increment * GetLevel(type);
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

    #region Public API — Operations

    /// <summary>Tăng 1 cấp cho PowerUp.</summary>
    public void Upgrade(PowerUpType type)
    {
        if (IsMaxLevel(type))
        {
            Debug.Log($"[PowerUpService] {type} already at max level ({GetMaxLevel(type)})");
            return;
        }

        int oldLevel = GetLevel(type);
        int newLevel = oldLevel + 1;
        SetLevel(type, newLevel);
    }

    /// <summary>Set level trực tiếp (dùng khi load save).</summary>
    public void SetLevel(PowerUpType type, int level)
    {
        int maxLevel = GetMaxLevel(type);
        int clampedLevel = Mathf.Clamp(level, 0, maxLevel);

        int oldLevel = GetLevel(type);
        if (oldLevel == clampedLevel) return;

        levels[type] = clampedLevel;

        Debug.Log($"[PowerUpService] {type}: {oldLevel} \u2192 {clampedLevel}");

        // Fire event
        OnPowerUpLevelChanged?.Invoke(type, oldLevel, clampedLevel);
        EventBus<PowerUpLevelChangedEvent>.Raise(new PowerUpLevelChangedEvent
        {
            Type = type,
            PreviousLevel = oldLevel,
            NewLevel = clampedLevel,
        });
    }

    /// <summary>Reset tất cả PowerUp về 0.</summary>
    public void ResetAll()
    {
        foreach (PowerUpType type in Enum.GetValues(typeof(PowerUpType)))
        {
            SetLevel(type, 0);
        }
        Debug.Log("[PowerUpService] All power-ups reset to level 0");
    }

    /// <summary>Kích hoạt tác dụng tức thời (Heal, ...).</summary>
    public void Activate(PowerUpType type)
    {
        if (!Definitions.TryGetValue(type, out var def))
            return;

        Debug.Log($"[PowerUpService] Activated {def.Icon} {type}");

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

    /// <summary>Lấy thông tin hiển thị của PowerUp.</summary>
    public static PowerUpDefinition GetDefinition(PowerUpType type)
    {
        return Definitions.TryGetValue(type, out var def) ? def : default;
    }

    /// <summary>Lấy tất cả categories.</summary>
    public static IEnumerable<PowerUpCategory> GetAllCategories()
    {
        yield return PowerUpCategory.Attack;
        yield return PowerUpCategory.Survival;
        yield return PowerUpCategory.Bonus;
    }

    /// <summary>Kiểm tra category của PowerUp.</summary>
    public static PowerUpCategory GetCategory(PowerUpType type)
    {
        return Definitions.TryGetValue(type, out var def) ? def.Category : PowerUpCategory.Attack;
    }

    #endregion
}

#region PowerUpDefinition

/// <summary>Định nghĩa cấu hình cho một PowerUp.</summary>
public struct PowerUpDefinition
{
    public string DisplayName;
    public string Icon;
    public string Description;
    public PowerUpCategory Category;
    public int MaxLevel;
    public float BaseValue;
    public float Increment; // Giá trị tăng thêm mỗi level
}

#endregion
