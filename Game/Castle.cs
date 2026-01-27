using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Game;
using System;

public class Castle : GameObject
{
    public Castle()
    {
        Scale = new Vector2(0.1f, 0.1f);
        Tag = "Castle";
        Position = GameConstants.CENTER;

        SetupComponents();
    }

    private void SetupComponents()
    {
        var renderer = AddComponent<SpriteRenderer>();
        renderer.Texture = ResourceManager.Instance.GetTexture("castle");

        if (renderer.Texture != null)
        {
            renderer.Origin = new Vector2(renderer.Texture.Width / 2f, renderer.Texture.Height / 2f);
        }

        var health = AddComponent<HealthComponent>();
        health.MaxHP = 100;
        health.Initialize();

        health.OnDamage += (damage) =>
        {
            System.Diagnostics.Debug.WriteLine($"Castle HP: {health.CurrentHP}/{health.MaxHP}");
        };

        health.OnDeath += () =>
        {
            System.Diagnostics.Debug.WriteLine($"Castle has been destroyed, game over!");
            Active = false;
        };
    }
}