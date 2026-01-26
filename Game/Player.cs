using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class Player : GameObject
{
    // Movement
    public float CurrentAngle { get; private set; } = 0f;
    public float RotationSpeed { get; set; } = 3f;  // radians per second

    // Animation settings
    public float IdleEyeOpenMultiplier { get; set; } = 5f;  

    // Shooting
    public float ShootCooldown { get; set; } = 0.3f;
    private float _shootTimer = 0f;

    // Animation
    private Animator _animator;
    private SpriteRenderer _renderer;

    // State
    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private const float ATTACK_DURATION = 0.4f;

    // Reference to scene
    private GameScene _scene;

    public Player()
    {
        Tag = "Player";
        Scale = new Vector2(0.3f, 0.3f);

        UpdatePositionOnRing();
        SetupRenderer();
        SetupAnimations();
    }

    public void SetScene(GameScene scene)
    {
        _scene = scene;
    }

    private void SetupRenderer()
    {
        _renderer = AddComponent<SpriteRenderer>();
    }

    private void SetupAnimations()
    {
        _animator = AddComponent<Animator>();

        var idleFrames = new List<Texture2D>
          {
              ResourceManager.Instance.GetTexture("P_idle_0"),
              ResourceManager.Instance.GetTexture("P_idle_1"),
              ResourceManager.Instance.GetTexture("P_idle_2"),
              ResourceManager.Instance.GetTexture("P_idle_3"),
              ResourceManager.Instance.GetTexture("P_idle_4")
          };
        var idleAnimation = new Animation(idleFrames, 0.15f);

        idleAnimation.SetFrameMultiplier(0, IdleEyeOpenMultiplier);
        _animator.AddAnimation("idle", idleAnimation);

        var attackFrames = new List<Texture2D>
          {
              ResourceManager.Instance.GetTexture("P_attack_1"),
              ResourceManager.Instance.GetTexture("P_attack_2"),
              ResourceManager.Instance.GetTexture("P_attack_3"),
              ResourceManager.Instance.GetTexture("P_attack_4")
          };
        var attackAnimation = new Animation(attackFrames, 0.1f);
        _animator.AddAnimation("attack", attackAnimation);

        _animator.Play("idle");

        if (idleFrames[0] != null)
        {
            _renderer.Origin = new Vector2(idleFrames[0].Width / 2f, idleFrames[0].Height / 2f);
        }
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // shoot cooldown
        if (_shootTimer > 0)
        {
            _shootTimer -= deltaTime;
        }

        HandleMovementInput(deltaTime);
        HandleAttackInput();
        HandleAttackState(deltaTime);
        UpdateAnimation();

        base.Update(gameTime);
    }

    private void HandleMovementInput(float deltaTime)
    {
        // A or Left Arrow = rotate counter-clockwise (ทวนเข็ม)
        // D or Right Arrow = rotate clockwise (ตามเข็ม)

        int direction = 0;

        if (InputManager.Instance.IsKeyDown(Keys.A) || InputManager.Instance.IsKeyDown(Keys.Left))
        {
            direction = -1;  // ทวนเข็มนาฬิกา
        }
        else if (InputManager.Instance.IsKeyDown(Keys.D) || InputManager.Instance.IsKeyDown(Keys.Right))
        {
            direction = 1;   // ตามเข็มนาฬิกา
        }

        if (direction != 0)
        {
            CurrentAngle += direction * RotationSpeed * deltaTime;
            UpdatePositionOnRing();
        }
    }

    private void HandleAttackInput()
    {
        // Mouse left click = shoot (hold for auto-fire)
        bool wantToShoot = Mouse.GetState().LeftButton == ButtonState.Pressed;

        if (wantToShoot && _shootTimer <= 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        _shootTimer = ShootCooldown;

        StartAttack();

        // Spawn projectile
        if (_scene != null)
        {
            Vector2 directionOutward = Position - GameConstants.CENTER;
            _scene.SpawnProjectile(Position, directionOutward);
        }
    }

    private void UpdatePositionOnRing()
    {
        float x = GameConstants.CENTER.X + (float)Math.Cos(CurrentAngle) * GameConstants.ORANGE_RADIUS;
        float y = GameConstants.CENTER.Y + (float)Math.Sin(CurrentAngle) * GameConstants.ORANGE_RADIUS;
        Position = new Vector2(x, y);

        Rotation = CurrentAngle - MathHelper.PiOver2;
    }

    private void StartAttack()
    {
        if (_isAttacking) return;

        _isAttacking = true;
        _attackTimer = ATTACK_DURATION;
        _animator.Play("attack");
    }

    private void HandleAttackState(float deltaTime)
    {
        if (_isAttacking)
        {
            _attackTimer -= deltaTime;
            if (_attackTimer <= 0)
            {
                _isAttacking = false;
                _animator.Play("idle");
            }
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

    public int GetCurrentLaneIndex()
    {
        float degrees = MathHelper.ToDegrees(CurrentAngle);
        if (degrees < 0) degrees += 360;
        return (int)(degrees / GameConstants.LANE_ANGLE_STEP) % GameConstants.LANE_COUNT;
    }

    public bool IsAttacking => _isAttacking;

    // Player กำลัง shoot cooldown อยู่หรือไม่
    public bool IsOnShootCooldown => _shootTimer > 0;

    /// เวลา shoot cooldown ที่เหลือ (วินาที)
    public float GetRemainingShootCooldown() => Math.Max(0f, _shootTimer);
}
