using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components;

class Animator : Component
{
    private readonly Dictionary<string, Animation> _animations = [];
    private Animation _currentAnimation;

    public void AddAnimation(string name, Animation animation)
    {
        _animations[name] = animation;
    }

    public void Play(string name)
    {
        if (_animations.TryGetValue(name, out var animation))
        {
            _currentAnimation = animation;
        }
    }

    public Animation GetCurrentAnimation()
    {
        return _currentAnimation;
    }

    public override void Update(GameTime gameTime)
    {
        _currentAnimation?.Update(gameTime);
    }
}