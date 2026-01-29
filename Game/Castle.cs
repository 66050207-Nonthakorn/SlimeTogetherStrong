using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Game;
using System;
using System.Collections.Generic;

public class Castle : GameObject
{
    // Animation
    private Animator _animator;
    private SpriteRenderer _renderer;
    public Castle()
    {
        Scale = new Vector2(0.5f, 0.5f);
        Tag = "Castle";
        Position = GameConstants.CENTER;

        SetupComponents();
        SetupRenderer();
        SetupAnimations();
    }

    private void SetupComponents()
    {
        var health = AddComponent<HealthComponent>();
        health.MaxHP = 1000;
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

    private void SetupRenderer()
    {
        _renderer = AddComponent<SpriteRenderer>();
    }

    private void SetupAnimations()
    {
        _animator = AddComponent<Animator>();

        // idle animation
        var idleFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Castle_idle_0"),
            ResourceManager.Instance.GetTexture("Castle_idle_1"),
            ResourceManager.Instance.GetTexture("Castle_idle_2"),
            ResourceManager.Instance.GetTexture("Castle_idle_3"),
            ResourceManager.Instance.GetTexture("Castle_idle_4")
        };
        var idleAnimation = new Animation(idleFrames, 0.2f);
        _animator.AddAnimation("idle", idleAnimation);

        _animator.Play("idle");
        if (idleFrames[0] != null)
        {
            _renderer.Origin = new Vector2(idleFrames[0].Width / 2f, idleFrames[0].Height / 2f);
        }
    }

    private void UpdateAnimation()
    {
        var currentAnimation = _animator.GetCurrentAnimation();
        if (currentAnimation?.CurrentFrame != null)
        {
            _renderer.Texture = currentAnimation.CurrentFrame;
        }
    }

    public override void Update(GameTime gameTime)
    {
        UpdateAnimation();
        base.Update(gameTime);
    }
}