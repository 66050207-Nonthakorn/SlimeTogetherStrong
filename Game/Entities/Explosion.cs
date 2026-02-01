using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Game.Constants;

namespace SlimeTogetherStrong.Game.Entities;

public class Explosion : GameObject
{
    private readonly Animator _animator;

    public Explosion(Vector2 position)
    {
        Position = position;
        Scale = new Vector2(GameConstants.PROJECTILE_SCALE, GameConstants.PROJECTILE_SCALE);

        _animator = AddComponent<Animator>();
        SetupAnimation();
    }

    private void SetupAnimation()
    {
        var explosionFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-1"),
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-2"),
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-3"),
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-4"),
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-5"),
            ResourceManager.Instance.GetTexture("Fireball/explosion/Fireball_Explosion-6")
        };

        var explosionAnimation = new Animation(explosionFrames, 0.08f);
        _animator.AddAnimation("explode", explosionAnimation);
        _animator.Play("explode");
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // ถ้า animation เล่นจบแล้ว ให้ลบ object ออก
        var anim = _animator.GetCurrentAnimation();
        if (anim != null)
        {
            // ตรวจสอบว่าเล่นถึง frame สุดท้ายแล้วหรือยัง
            if (anim.CurrentFrameIndex >= anim.FrameCount - 1)
            {
                Active = false;
            }
        }
    }
}
