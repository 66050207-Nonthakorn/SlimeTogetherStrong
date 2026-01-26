using System;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components;

public class HealthComponent : Component
{
    public int MaxHP { get; set; } = 100;
    public int CurrentHP { get; private set; }

    // Events
    public event Action OnDeath;
    public event Action<int> OnDamage;  // parameter = damage amount
    public event Action<int> OnHeal;    // parameter = heal amount

    public override void Initialize()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead()) return;

        int actualDamage = Math.Min(damage, CurrentHP);
        CurrentHP -= actualDamage;

        OnDamage?.Invoke(actualDamage);

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead()) return;

        int actualHeal = Math.Min(amount, MaxHP - CurrentHP);
        CurrentHP += actualHeal;

        if (actualHeal > 0)
        {
            OnHeal?.Invoke(actualHeal);
        }
    }

    public bool IsDead()
    {
        return CurrentHP <= 0;
    }

    /// <summary>
    /// ยังมีชีวิตอยู่หรือไม่ (ตรงข้ามกับ IsDead)
    /// </summary>
    public bool IsAlive => CurrentHP > 0;

    public float GetHealthPercentage()
    {
        return (float)CurrentHP / MaxHP;
    }

    public void SetMaxHP(int maxHP, bool healToFull = true)
    {
        MaxHP = maxHP;
        if (healToFull)
        {
            CurrentHP = MaxHP;
        }
        else
        {
            CurrentHP = Math.Min(CurrentHP, MaxHP);
        }
    }
}
