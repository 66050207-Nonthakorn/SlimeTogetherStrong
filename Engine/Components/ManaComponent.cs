using System;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components;

public class ManaComponent : Component
{
    public int MaxMana { get; set; } = 100;
    public int CurrentMana { get; private set; }

    // Mana regeneration
    public float ManaRegenRate { get; set; } = 5f; // Mana per second
    private float _regenTimer = 0f;

    // Events
    public event Action<int> OnManaUsed;    // parameter = mana used
    public event Action<int> OnManaRegen;   // parameter = mana regenerated

    public override void Initialize()
    {
        CurrentMana = MaxMana;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Regenerate mana over time
        if (CurrentMana < MaxMana)
        {
            _regenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_regenTimer >= 1f)
            {
                _regenTimer = 0f;
                RegenerateMana((int)ManaRegenRate);
            }
        }
    }

    public bool UseMana(int amount)
    {
        if (CurrentMana >= amount)
        {
            CurrentMana -= amount;
            OnManaUsed?.Invoke(amount);
            return true;
        }
        return false;
    }

    public void RegenerateMana(int amount)
    {
        int actualRegen = Math.Min(amount, MaxMana - CurrentMana);
        CurrentMana += actualRegen;

        if (actualRegen > 0)
        {
            OnManaRegen?.Invoke(actualRegen);
        }
    }

    public float GetManaPercentage()
    {
        return (float)CurrentMana / MaxMana;
    }

    public void SetMaxMana(int maxMana, bool fillToFull = true)
    {
        MaxMana = maxMana;
        if (fillToFull)
        {
            CurrentMana = MaxMana;
        }
        else
        {
            CurrentMana = Math.Min(CurrentMana, MaxMana);
        }
    }
}
