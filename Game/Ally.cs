using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;
using SlimeTogetherStrong.Game;

public class Ally : GameObject
{
    private AllyState state = AllyState.Idle;
    private Enemy currentTarget;
    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private const float ATTACK_DURATION = 1f;


    // Animation
    private Animator _animator;
    private SpriteRenderer _renderer;
    public float IdleEyeOpenMultiplier { get; set; } = 5f;

    // Reference to scene and lane
    public GameScene _scene;
    public int SlotIndex;
    public LaneData ParentLane;

    public Ally()
    {
        Tag = "Ally";
        Scale = new Vector2(0.2f, 0.2f);

        SetupOtherComponents();
        SetupRenderer();
        SetupAnimations();
    }

    public void SetupOtherComponents()
    {
        var collider = AddComponent<CircleCollider>();
        collider.Radius = 30f;

        var health = AddComponent<HealthComponent>();
        health.MaxHP = 100;
        health.Initialize();

        health.OnDamage += (damage) => {};

        health.OnDeath += () =>
        {
            Active = false;
        };

        var combat = AddComponent<CombatComponent>();
        combat.Damage = 15;
        combat.AttackSpeed = 1.5f;

        combat.OnAttack += (target) =>
        {
            _animator.Play("attack");
        };

        // Add health bar with green color
        var healthBar = AddComponent<HealthBar>();
        healthBar.FillColor = Color.Green;
        healthBar.BackgroundColor = new Color(20, 40, 20);
        healthBar.BorderColor = Color.DarkGreen;
        healthBar.Size = new Vector2(50, 6);
        healthBar.Offset = new Vector2(10, -20);
    }

    public void SetScene(GameScene scene)
    {
        _scene = scene;
    }

    public void ApplyBonusStats(int bonusHP, int bonusDamage)
    {
        var health = GetComponent<HealthComponent>();
        if (health != null)
        {
            health.MaxHP += bonusHP;
            health.Heal(bonusHP);
        }

        var combat = GetComponent<CombatComponent>();
        if (combat != null)
        {
            combat.Damage += bonusDamage;
        }
    }

    private void SetupRenderer()
    {
        _renderer = AddComponent<SpriteRenderer>();
    }

    private void SetupAnimations()
    {
        _animator = AddComponent<Animator>();

        // Idle Animation
        var idleFrames = new List<Texture2D>
          {
              ResourceManager.Instance.GetTexture("A_idle_0"),
              ResourceManager.Instance.GetTexture("A_idle_1"),
              ResourceManager.Instance.GetTexture("A_idle_2"),
              ResourceManager.Instance.GetTexture("A_idle_3")
          };
        var idleAnimation = new Animation(idleFrames, 0.2f);

        idleAnimation.SetFrameMultiplier(0, IdleEyeOpenMultiplier);
        _animator.AddAnimation("idle", idleAnimation);

        // Attack Animation
        var attackFrames = new List<Texture2D>
          {
              ResourceManager.Instance.GetTexture("A_attack_1"),
              ResourceManager.Instance.GetTexture("A_attack_2"),
              ResourceManager.Instance.GetTexture("A_attack_3"),
              ResourceManager.Instance.GetTexture("A_attack_4"),
              ResourceManager.Instance.GetTexture("A_attack_5")
          };
        var attackAnimation = new Animation(attackFrames, 0.1f);
        _animator.AddAnimation("attack", attackAnimation);

        _animator.Play("idle");

        if (idleFrames[0] != null)
        {
            _renderer.Origin = new Vector2(idleFrames[0].Width / 2f, idleFrames[0].Height / 2f);
        }
    }

    public void SetupLane(LaneData lane, int slotIndex)
    {
        ParentLane = lane;
        SlotIndex = slotIndex;
    }

    private void FindTarget()
    {
        Enemy nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null && Vector2.Distance(Position, nearestEnemy.Position) < 30f) {
            currentTarget = nearestEnemy;
            state = AllyState.Attacking;
            return;
        }
    }

    private Enemy FindNearestEnemy() {
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        if (ParentLane == null)
            return null;

        foreach (var enemy in ParentLane.Enemies) {
            if (enemy.Active) {
                float distance = Vector2.Distance(Position, enemy.Position);
                if (distance < nearestDistance) {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
    }

    private void StartAttack()
    {
        if (_isAttacking) return;

        _isAttacking = true;
        _attackTimer = ATTACK_DURATION;
        _animator.Play("attack");
    }


    private void UpdateAnimation()
    {
        var currentAnimation = _animator.GetCurrentAnimation();
        if (currentAnimation?.CurrentFrame != null)
        {
            _renderer.Texture = currentAnimation.CurrentFrame;
        }
    }

    private void UpdateAttackState(float deltaTime)
    {
        if (!_isAttacking) return;

            _attackTimer -= deltaTime;
            if (_attackTimer <= 0)
        {
            _isAttacking = false;
            _animator.Play("idle");
        }
    }


    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch (state)
        {
            case AllyState.Idle:
                FindTarget();
                break;
            case AllyState.Attacking:
                if (currentTarget == null ||
                    currentTarget.GetComponent<HealthComponent>()?.IsDead() == true)
                {
                    state = AllyState.Idle;
                    currentTarget = null;
                    _animator.Play("idle");
                    break;
                }

                var combat = GetComponent<CombatComponent>();

                if (combat.CanAttack())
                {
                    combat.Attack(currentTarget);
                    StartAttack(); 
                }

                break;
                

        }

        float forwardOffset = GameConstants.BLUE_RADIUS + SlotIndex * GameConstants.FORWARD_SPACING;

        float sideOffset = (SlotIndex % 2 == 0 ? -1 : 1) * GameConstants.SIDE_SPACING;

        Position =
            ParentLane.EndPoint
            + ParentLane.Direction * forwardOffset
            + ParentLane.Perpendicular * sideOffset;

        
        UpdateAttackState(deltaTime);
        UpdateAnimation();
        base.Update(gameTime);
    }
}
