using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityServiceLocator;

/*
 * PowerUpTest — MonoBehaviour test cho hệ thống PowerUp.
 * Gắn component này vào bất kỳ GameObject nào trong scene,
 * dùng [Button] trong Inspector để test.
 */

public class PowerUpTest : MonoBehaviour
{
    [Header("Test Settings")]
    public PowerUpType testType = PowerUpType.Stronger;
    public int testLevel = 3;
    public int testLoopCount = 5;

    private IPowerUpService powerUp;

    private IPowerUpService PowerUp
    {
        get
        {
            if (powerUp == null)
                powerUp = ServiceLocator.Global.Get<IPowerUpService>();
            return powerUp;
        }
    }

    // ============================================================
    //  QUERIES
    // ============================================================

    [Button("Test - Log All Levels")]
    private void LogAllLevels()
    {
        var svc = PowerUp;
        if (svc == null)
        {
            Debug.LogError("[Test] PowerUpService is null!");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("=== All PowerUp Levels ===");

        foreach (var kvp in svc.AllLevels)
        {
            var data = svc.GetData(kvp.Key);
            string icon = !string.IsNullOrEmpty(data?.Description) && data.Description != "(data not loaded)"
                ? data.POWERUPCATEGORY.ToString() : "?";
            sb.AppendLine($"{icon} {kvp.Key,-15} Lv.{kvp.Value,-2} | " +
                          $"Stat: {svc.GetEffectValue(kvp.Key),-8:F2} | " +
                          $"Cost: {svc.GetUpgradeCost(kvp.Key),-6} | " +
                          $"MaxLv: {svc.GetMaxLevel(kvp.Key)}");
        }

        Debug.Log(sb.ToString());
    }

    [Button("Test - Log Category Attack")]
    private void LogAttackCategory()
    {
        LogCategory(PowerUpCategory.Attack);
    }

    [Button("Test - Log Category Survival")]
    private void LogSurvivalCategory()
    {
        LogCategory(PowerUpCategory.Survival);
    }

    [Button("Test - Log Category Bonus")]
    private void LogBonusCategory()
    {
        LogCategory(PowerUpCategory.Bonus);
    }

    private void LogCategory(PowerUpCategory category)
    {
        var svc = PowerUp;
        if (svc == null)
        {
            Debug.LogError("[Test] PowerUpService is null!");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"=== {category} ===");

        foreach (var type in svc.GetPowerUpsInCategory(category))
        {
            var data = svc.GetData(type);
            sb.AppendLine($"{data?.POWERUPCATEGORY ?? PowerUpCategory.Attack} {type,-15} Lv.{svc.GetLevel(type),-2} | " +
                          $"Value: {svc.GetEffectValue(type),-8:F2} | " +
                          $"Cost: {svc.GetUpgradeCost(type),-6}");
        }

        Debug.Log(sb.ToString());
    }

    [Button("Test - Log TestType Info")]
    private void LogTestTypeInfo()
    {
        var svc = PowerUp;
        if (svc == null)
        {
            Debug.LogError("[Test] PowerUpService is null!");
            return;
        }

        var data = svc.GetData(testType);
        bool dataReady = data != null && data.Description != "(data not loaded)";

        var sb = new StringBuilder();
        sb.AppendLine($"=== {data?.POWERUPCATEGORY ?? PowerUpCategory.Attack} {testType} ===");
        sb.AppendLine($"  Group:      {data?.POWERUPCATEGORY ?? PowerUpCategory.Attack}");
        sb.AppendLine($"  BaseValue:  {data?.Basevalue ?? 0}");
        sb.AppendLine($"  PerLevel:   {data?.Perlevel ?? 0}");
        sb.AppendLine($"  MaxLevel:   {data?.Maxlevel ?? 0}");
        sb.AppendLine($"  BaseCost:   {data?.Basecost ?? 0}");
        sb.AppendLine($"  CostGrowth: {data?.Costgrowth ?? 0}");
        sb.AppendLine($"  Desc:       {data?.Description ?? "N/A"}");
        sb.AppendLine($"--- Current ---");
        sb.AppendLine($"  Level:     {svc.GetLevel(testType)}");
        sb.AppendLine($"  Stat:      {svc.GetEffectValue(testType):F2}");
        sb.AppendLine($"  Max:       {svc.IsMaxLevel(testType)}");
        sb.AppendLine($"  Unlocked:  {svc.IsUnlocked(testType)}");
        sb.AppendLine($"  Cost:      {svc.GetUpgradeCost(testType)}");

        if (dataReady)
        {
            sb.AppendLine($"--- Preview ---");
            for (int lv = 0; lv <= data.Maxlevel; lv++)
            {
                float val = data.Basevalue + data.Perlevel * lv;
                int cost = lv >= data.Maxlevel ? 0 :
                    Mathf.RoundToInt(data.Basecost * Mathf.Pow(data.Costgrowth, lv));
                sb.AppendLine($"  Lv.{lv} → Value: {val,-8:F2} | Cost: {cost,-6}");
            }
        }

        Debug.Log(sb.ToString());
    }

    // ============================================================
    //  OPERATIONS
    // ============================================================

    [Button("Test - Upgrade TestType 1 level")]
    private void UpgradeOneLevel()
    {
        bool ok = PowerUp.TryUpgrade(testType);
        Debug.Log($"[Test] TryUpgrade({testType}) → {(ok ? "OK" : "FAIL (max level)")}");
        LogTestTypeInfo();
    }

    [Button("Test - Upgrade TestType to Max")]
    private void UpgradeToMax()
    {
        var svc = PowerUp;
        int count = 0;
        while (svc.TryUpgrade(testType))
        {
            count++;
        }
        Debug.Log($"[Test] Upgraded {testType} to max ({count} steps)");
        LogTestTypeInfo();
    }

    [Button("Test - Set TestType to Level 0 (Reset)")]
    private void ResetTestType()
    {
        PowerUp.SetLevel(testType, 0);
        Debug.Log($"[Test] Reset {testType} to level 0");
        LogTestTypeInfo();
    }

    [Button("Test - Set TestType to TestLevel")]
    private void SetToTestLevel()
    {
        PowerUp.SetLevel(testType, testLevel);
        Debug.Log($"[Test] Set {testType} to level {testLevel}");
        LogTestTypeInfo();
    }

    [Button("Test - Reset All")]
    private void ResetAll()
    {
        PowerUp.ResetAll();
        Debug.Log("[Test] All power-ups reset");
        LogAllLevels();
    }

    [Button("Test - Loop Upgrade All Types 1 Level")]
    private void LoopUpgradeAll()
    {
        var svc = PowerUp;
        int totalUpgraded = 0;

        foreach (PowerUpType type in System.Enum.GetValues(typeof(PowerUpType)))
        {
            if (svc.TryUpgrade(type))
                totalUpgraded++;
        }

        Debug.Log($"[Test] Upgraded {totalUpgraded} power-ups by 1 level");
        LogAllLevels();
    }

    [Button("Test - Loop Upgrade All Types N Times")]
    private void LoopUpgradeAllNTimes()
    {
        var svc = PowerUp;
        int totalUpgraded = 0;

        for (int i = 0; i < testLoopCount; i++)
        {
            foreach (PowerUpType type in System.Enum.GetValues(typeof(PowerUpType)))
            {
                if (svc.TryUpgrade(type))
                    totalUpgraded++;
            }
        }

        Debug.Log($"[Test] {totalUpgraded} total upgrades ({testLoopCount} loops)");
        LogAllLevels();
    }

    // ============================================================
    //  ACTIVATE
    // ============================================================

    [Button("Test - Activate TestType (Heal...)")]
    private void ActivateTestType()
    {
        PowerUp.Activate(testType);
        Debug.Log($"[Test] Activated {testType}");
    }

    // ============================================================
    //  COST TABLE
    // ============================================================

    [Button("Test - Log Full Cost Table for TestType")]
    private void LogFullCostTable()
    {
        var data = PowerUp.GetData(testType);
        var sb = new StringBuilder();
        sb.AppendLine($"=== Cost Table: {data.POWERUPCATEGORY} {testType} ===");
        sb.AppendLine($"  BaseCost={data.Basecost}, CostGrowth={data.Costgrowth}");

        for (int lv = 0; lv <= data.Maxlevel; lv++)
        {
            int cost = lv >= data.Maxlevel ? 0 :
                Mathf.RoundToInt(data.Basecost * Mathf.Pow(data.Costgrowth, lv));
            float val = data.Basevalue + data.Perlevel * lv;
            sb.AppendLine($"  Lv.{lv,-2} → Value: {val,-8:F2} | Cost: {cost,-6}{(lv >= data.Maxlevel ? " (MAX)" : "")}");
        }

        Debug.Log(sb.ToString());
    }

    [Button("Log LevelPreview Data")]
    private void LogLevelPreview()
    {
        var preview = Resources.Load<LevelPreview>("Data/LevelPreview");
        if (preview == null || preview.dataArray == null)
        {
            Debug.LogWarning("[Test] LevelPreview asset not found at Resources/Data/LevelPreview");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("=== LevelPreview Data ===");
        foreach (var data in preview.dataArray)
        {
            sb.AppendLine($"  {data.Upgradecost,-15} Lv.{data.Level,-2} → Value: {data.Value,-8:F2} | Cost: {data.Upgradecost,-6}");
        }

        Debug.Log(sb.ToString());
    }
}
