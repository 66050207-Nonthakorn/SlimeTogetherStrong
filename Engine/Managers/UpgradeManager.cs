using System;
using System.Collections.Generic;

namespace SlimeTogetherStrong.Engine.Managers;

public enum UpgradeType
{
    CastleMaxHP,
    CastleHeal,
    PlayerMaxMana,
    PlayerManaRegen,
    PlayerDamage,
    PlayerAttackSpeed,
    AllyMaxHP,
    AllyDamage
}

public class UpgradeData
{
    public UpgradeType Type { get; set; }
    public string EntityName { get; set; }
    public string StatsName { get; set; }
    public int Value { get; set; }
    public string EntityTextureKey { get; set; }
    public string StatsIconKey { get; set; }
}

public class UpgradeManager
{
    public static UpgradeManager Instance { get; private set; }

    // Stored upgrade bonuses
    public int BonusCastleMaxHP { get; private set; } = 0;
    public int BonusPlayerMaxMana { get; private set; } = 0;
    public float BonusPlayerManaRegen { get; private set; } = 0f;
    public int BonusPlayerDamage { get; private set; } = 0;
    public float BonusPlayerAttackSpeed { get; private set; } = 0f; // Reduces cooldown
    public int BonusAllyMaxHP { get; private set; } = 0;
    public int BonusAllyDamage { get; private set; } = 0;

    // All possible upgrades
    private List<UpgradeData> _allUpgrades;
    private Random _random = new Random();

    public UpgradeManager()
    {
        Instance = this;
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        _allUpgrades = new List<UpgradeData>
        {
            // Castle upgrades
            new UpgradeData
            {
                Type = UpgradeType.CastleMaxHP,
                EntityName = "Castle",
                StatsName = "Max HP",
                Value = 200,
                EntityTextureKey = "Castle_idle_0",
                StatsIconKey = "Health_Icon"
            },
            new UpgradeData
            {
                Type = UpgradeType.CastleHeal,
                EntityName = "Castle",
                StatsName = "Heal",
                Value = 150,
                EntityTextureKey = "Castle_idle_0",
                StatsIconKey = "Health_Icon"
            },

            // Player upgrades
            new UpgradeData
            {
                Type = UpgradeType.PlayerMaxMana,
                EntityName = "Player",
                StatsName = "Max Mana",
                Value = 20,
                EntityTextureKey = "P_idle_0",
                StatsIconKey = "Mana_Icon"
            },
            new UpgradeData
            {
                Type = UpgradeType.PlayerManaRegen,
                EntityName = "Player",
                StatsName = "Mana Regen",
                Value = 2, // +2 mana/sec
                EntityTextureKey = "P_idle_0",
                StatsIconKey = "Mana_Icon"
            },
            new UpgradeData
            {
                Type = UpgradeType.PlayerDamage,
                EntityName = "Player",
                StatsName = "Damage",
                Value = 5,
                EntityTextureKey = "P_idle_0",
                StatsIconKey = "Health_Icon" // ใช้ health icon แทน damage icon
            },
            new UpgradeData
            {
                Type = UpgradeType.PlayerAttackSpeed,
                EntityName = "Player",
                StatsName = "Attack Speed",
                Value = 10, // 10% faster
                EntityTextureKey = "P_idle_0",
                StatsIconKey = "Mana_Icon"
            },

            // Ally upgrades
            new UpgradeData
            {
                Type = UpgradeType.AllyMaxHP,
                EntityName = "Ally",
                StatsName = "Max HP",
                Value = 25,
                EntityTextureKey = "A_idle_0",
                StatsIconKey = "Health_Icon"
            },
            new UpgradeData
            {
                Type = UpgradeType.AllyDamage,
                EntityName = "Ally",
                StatsName = "Damage",
                Value = 5,
                EntityTextureKey = "A_idle_0",
                StatsIconKey = "Health_Icon"
            }
        };
    }

    public List<UpgradeData> GetRandomUpgrades(int count = 3)
    {
        var result = new List<UpgradeData>();
        var available = new List<UpgradeData>(_allUpgrades);

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int index = _random.Next(available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }

        return result;
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.CastleMaxHP:
                BonusCastleMaxHP += upgrade.Value;
                System.Console.WriteLine($"Castle Max HP +{upgrade.Value} (Total bonus: {BonusCastleMaxHP})");
                break;

            case UpgradeType.CastleHeal:
                // Heal is applied immediately, not stored
                System.Console.WriteLine($"Castle healed for {upgrade.Value}");
                break;

            case UpgradeType.PlayerMaxMana:
                BonusPlayerMaxMana += upgrade.Value;
                System.Console.WriteLine($"Player Max Mana +{upgrade.Value} (Total bonus: {BonusPlayerMaxMana})");
                break;

            case UpgradeType.PlayerManaRegen:
                BonusPlayerManaRegen += upgrade.Value;
                System.Console.WriteLine($"Player Mana Regen +{upgrade.Value} (Total bonus: {BonusPlayerManaRegen})");
                break;

            case UpgradeType.PlayerDamage:
                BonusPlayerDamage += upgrade.Value;
                System.Console.WriteLine($"Player Damage +{upgrade.Value} (Total bonus: {BonusPlayerDamage})");
                break;

            case UpgradeType.PlayerAttackSpeed:
                BonusPlayerAttackSpeed += upgrade.Value / 100f; // Convert to percentage
                System.Console.WriteLine($"Player Attack Speed +{upgrade.Value}% (Total bonus: {BonusPlayerAttackSpeed * 100}%)");
                break;

            case UpgradeType.AllyMaxHP:
                BonusAllyMaxHP += upgrade.Value;
                System.Console.WriteLine($"Ally Max HP +{upgrade.Value} (Total bonus: {BonusAllyMaxHP})");
                break;

            case UpgradeType.AllyDamage:
                BonusAllyDamage += upgrade.Value;
                System.Console.WriteLine($"Ally Damage +{upgrade.Value} (Total bonus: {BonusAllyDamage})");
                break;
        }
    }

    public void Reset()
    {
        BonusCastleMaxHP = 0;
        BonusPlayerMaxMana = 0;
        BonusPlayerManaRegen = 0f;
        BonusPlayerDamage = 0;
        BonusPlayerAttackSpeed = 0f;
        BonusAllyMaxHP = 0;
        BonusAllyDamage = 0;
    }
}
