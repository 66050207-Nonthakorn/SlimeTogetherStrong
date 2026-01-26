using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components;

public class Animator : Component
{
    private readonly Dictionary<string, Animation> _animations = [];
    private Animation _currentAnimation;
    private SpriteRenderer _renderer;

    public override void Initialize()
    {
        base.Initialize();
        // หา SpriteRenderer ใน GameObject เดียวกัน
        _renderer = GameObject.GetComponent<SpriteRenderer>();

        // ถ้าไม่มี SpriteRenderer ให้สร้างใหม่
        if (_renderer == null)
        {
            _renderer = GameObject.AddComponent<SpriteRenderer>();
        }
    }

    public void AddAnimation(string name, Animation animation)
    {
        _animations[name] = animation;
    }

    public void Play(string name)
    {
        if (_animations.TryGetValue(name, out var animation))
        {
            _currentAnimation = animation;
            _currentAnimation.Reset();
        }
    }

    public Animation GetCurrentAnimation()
    {
        return _currentAnimation;
    }

    public override void Update(GameTime gameTime)
    {
        _currentAnimation?.Update(gameTime);

        // อัพเดท texture ใน SpriteRenderer
        if (_currentAnimation != null && _renderer != null)
        {
            _renderer.Texture = _currentAnimation.CurrentFrame;

            // ตั้ง Origin ให้อยู่กึ่งกลาง texture
            if (_renderer.Texture != null)
            {
                _renderer.Origin = new Microsoft.Xna.Framework.Vector2(
                    _renderer.Texture.Width / 2f,
                    _renderer.Texture.Height / 2f
                );
            }
        }
    }
}