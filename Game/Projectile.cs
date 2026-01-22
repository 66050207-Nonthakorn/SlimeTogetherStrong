using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class Projectile : GameObject
{
    public Vector2 Direction { get; private set; }
    public float Speed { get; set; } = 400f;
    public int Damage { get; set; } = 10;

    private SpriteRenderer _renderer;

    public Projectile(Vector2 startPosition, Vector2 direction)
    {
        Tag = "Projectile";
        Position = startPosition;
        Direction = Vector2.Normalize(direction);
        Scale = new Vector2(0.05f, 0.05f);


        SetupRenderer();
    }

    private void SetupRenderer()
    {
        _renderer = AddComponent<SpriteRenderer>();

        var texture = ResourceManager.Instance.GetTexture("bullet");

        if (texture == null)
        {
            System.Diagnostics.Debug.WriteLine("bullet texture is null");
        }

        _renderer.Texture = texture;

        if (_renderer.Texture != null)
        {
            _renderer.Origin = new Vector2(_renderer.Texture.Width / 2f, _renderer.Texture.Height / 2f);
        }
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += Direction * Speed * deltaTime;

        float distanceToCenter = Vector2.Distance(Position, GameConstants.CENTER);
        if (distanceToCenter < 50f)
        {
            Active = false;
        }

        if (Position.X < -50 || Position.X > GameConstants.SCREEN_WIDTH + 50 ||
            Position.Y < -50 || Position.Y > GameConstants.SCREEN_HEIGHT + 50)
        {
            Active = false;
        }

        base.Update(gameTime);
    }
}
