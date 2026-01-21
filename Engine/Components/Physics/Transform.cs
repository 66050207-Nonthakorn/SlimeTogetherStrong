using System;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components.Physics;

public class Transform : Component
{
    public Vector2 Forward
        => new((float)Math.Cos(base.GameObject.Rotation), (float)Math.Sin(base.GameObject.Rotation));

    public void Translate(Vector2 distance)
    {
        base.GameObject.Position += distance;
    }
    
    public void Rotate(float amount)
    {
        base.GameObject.Rotation += amount;
    }
}