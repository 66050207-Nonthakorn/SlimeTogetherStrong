using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class Projectile : GameObject
{
    public Vector2 Direction { get; private set; }
    public float Speed { get; set; } = GameConstants.PROJECTILE_SPEED;
    public int Damage { get; set; } = GameConstants.PROJECTILE_DAMAGE;

    private Animator _animator;

    public Projectile(Vector2 startPosition, Vector2 direction)
    {
        Tag = "Projectile";
        Position = startPosition;
        Direction = Vector2.Normalize(direction);
        Scale = new Vector2(GameConstants.PROJECTILE_SCALE, GameConstants.PROJECTILE_SCALE);

        SetupRenderer();
        SetupCollider();
    }

    private void SetupCollider()
    {
        var collider = AddComponent<CircleCollider>();
        collider.Radius = GameConstants.PROJECTILE_COLLIDER_RADIUS;
        collider.IsTrigger = true;
    }

    private void SetupRenderer()
    {
        _animator = AddComponent<Animator>();

        var frame1 = ResourceManager.Instance.GetTexture("Fireball/attack/Ranged_Attack-1");
        var frame2 = ResourceManager.Instance.GetTexture("Fireball/attack/Ranged_Attack-2");

        // Debug: ตรวจสอบว่าโหลด texture ได้หรือเปล่า
        if (frame1 == null || frame2 == null)
        {
            System.Diagnostics.Debug.WriteLine($"Fireball texture not found! frame1={frame1}, frame2={frame2}");
        }

        var attackFrames = new List<Texture2D> { frame1, frame2 };

        var attackAnimation = new Animation(attackFrames, 0.1f);
        _animator.AddAnimation("attack", attackAnimation);
        _animator.Play("attack");

        // คำนวณมุมหมุนจาก Direction
        // ภาพลูกไฟหัวชี้ขึ้นบน ต้องบวก 90° (π/2) ให้หัวชี้ไปทาง Direction
        Rotation = (float)Math.Atan2(Direction.Y, Direction.X) + MathHelper.PiOver2;
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
