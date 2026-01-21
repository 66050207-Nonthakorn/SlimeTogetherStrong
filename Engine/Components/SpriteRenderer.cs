using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeTogetherStrong.Engine.Components;

public class SpriteRenderer : Component
{
    public Texture2D Texture { get; set; }
    public Color Tint { get; set; } = Color.White;
    public Vector2 Origin { get; set; }
    public float LayerDepth { get; set; }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null) return;
        spriteBatch.Draw(Texture, base.GameObject.Position, null, Tint, 
            base.GameObject.Rotation, Origin, base.GameObject.Scale, SpriteEffects.None, LayerDepth);
    }
}