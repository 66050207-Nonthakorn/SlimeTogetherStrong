using System;

namespace SlimeTogetherStrong.Engine.Managers;

public class XPManager
{
    public int CurrentXP { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;
    public int XPForNextLevel { get; private set; } = 100;

    public event Action<int> OnXPChanged;
    public event Action<int> OnLevelUp;

    public XPManager() { }

    public void AddXP(int amount)
    {
        CurrentXP += amount;
        OnXPChanged?.Invoke(CurrentXP);

        // Check for level up
        while (CurrentXP >= XPForNextLevel)
        {
            LevelUp();
        }
    }

    // dummy level up logic
    private void LevelUp()
    {
        CurrentXP -= XPForNextLevel;
        CurrentLevel++;
        
        // XP requirement increases by 50 per level
        XPForNextLevel = 100 + (CurrentLevel - 1) * 50;
        
        OnLevelUp?.Invoke(CurrentLevel);
        OnXPChanged?.Invoke(CurrentXP);
    }

    public float GetXPPercentage()
    {
        return (float)CurrentXP / XPForNextLevel;
    }

    public void Reset()
    {
        CurrentXP = 0;
        CurrentLevel = 1;
        XPForNextLevel = 100;
    }
}
