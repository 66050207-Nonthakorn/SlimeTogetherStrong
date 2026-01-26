using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeTogetherStrong.Engine.Components;

public class Animation : Component
{
    public Texture2D CurrentFrame { get; private set; }
    public float DefaultFrameDuration { get; set; }
    public int CurrentFrameIndex => _currentFrameIndex;
    public int FrameCount => _frames.Count;

    private readonly List<Texture2D> _frames;
    private readonly List<float> _frameDurations;  // duration ของแต่ละ frame
    private float _timer = 0f;
    private int _currentFrameIndex = 0;

    public Animation(List<Texture2D> frames, float frameDuration)
    {
        _frames = frames;
        DefaultFrameDuration = frameDuration;

        _frameDurations = new List<float>();
        for (int i = 0; i < frames.Count; i++)
        {
            _frameDurations.Add(frameDuration);
        }

        CurrentFrame = frames.Count > 0 ? frames[0] : null;
    }

    public Animation(List<Texture2D> frames, List<float> frameDurations)
    {
        _frames = frames;
        _frameDurations = frameDurations;
        DefaultFrameDuration = frameDurations.Count > 0 ? frameDurations[0] : 0.1f;

        while (_frameDurations.Count < _frames.Count)
        {
            _frameDurations.Add(DefaultFrameDuration);
        }

        CurrentFrame = frames.Count > 0 ? frames[0] : null;
    }

    public void SetFrameDuration(int frameIndex, float duration)
    {
        if (frameIndex >= 0 && frameIndex < _frameDurations.Count)
        {
            _frameDurations[frameIndex] = duration;
        }
    }

    public void SetFrameMultiplier(int frameIndex, float multiplier)
    {
        if (frameIndex >= 0 && frameIndex < _frameDurations.Count)
        {
            _frameDurations[frameIndex] = DefaultFrameDuration * multiplier;
        }
    }

    public float GetFrameDuration(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < _frameDurations.Count)
        {
            return _frameDurations[frameIndex];
        }
        return DefaultFrameDuration;
    }

    public void Reset()
    {
        _currentFrameIndex = 0;
        _timer = 0f;
        CurrentFrame = _frames.Count > 0 ? _frames[0] : null;
    }

    public override void Update(GameTime gameTime)
    {
        if (_frames.Count == 0)
            return;

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float currentDuration = _frameDurations[_currentFrameIndex];

        if (_timer >= currentDuration)
        {
            _timer -= currentDuration;
            _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Count;
            CurrentFrame = _frames[_currentFrameIndex];
        }
    }
}
