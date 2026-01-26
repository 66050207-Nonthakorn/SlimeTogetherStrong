using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Game;
using System;

public class Castle : GameObject
{
    public int MaxHP;
    public int CurrentHP;

    public event Action OnDestroyed;

    public Castle(Vector2 position, int maxHP)
    {
        Position = GameConstants.CENTER;
        MaxHP = maxHP;
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Active = false;
            OnDestroyed?.Invoke();
        }
    }

    public bool IsDestroyed()
    {
        return CurrentHP <= 0;
    }
}