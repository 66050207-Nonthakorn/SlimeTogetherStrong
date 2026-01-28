using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class Enemy : GameObject {
    private float _moveSpeed = 50f;
    private EnemyState state = EnemyState.Moving;
    private GameObject currentTarget;

    private bool _isAttacking { get; set; } = false;
    private float _attackTimer = 0f;
    private const float ATTACK_DURATION = 1f;

    // Animation
    private Animator _animator;
    private SpriteRenderer _renderer;

    // Reference to scene and lane
    private GameScene _scene;
    private LaneData _parentLane;
    private int _slotIndex;

    public Enemy()
    {
        Tag = "Enemy";
        Scale = new Vector2(0.2f, 0.2f);

        SetupOtherComponents();
        SetupRenderer();
        SetupAnimations();
    }

    private void SetupOtherComponents()
    {
        var collider = AddComponent<CircleCollider>();
        collider.Radius = 30f;

        var health = AddComponent<HealthComponent>();
        health.MaxHP = 40;
        health.Initialize();

        health.OnDamage += (damage) => {};

        health.OnDeath += () =>
        {
            System.Diagnostics.Debug.WriteLine($"Enemy killed.");
            Active = false;

            for (int i = 0; i < _parentLane.Enemies.Count; i++)
            {
                _parentLane.Enemies[i].SetSlotIndex(i);
            }
        };

        var combat = AddComponent<CombatComponent>();
        
        combat.OnAttack += (target) =>
        {
            _animator.Play("attack");
        };
    }

    public void SetScene(GameScene scene)
    {
        _scene = scene;
    }

    public void SetParentLane(LaneData lane, int slotIndex)
    {
        _parentLane = lane;
        _slotIndex = slotIndex;
    }

    private void SetupRenderer()
    {
        _renderer = AddComponent<SpriteRenderer>();
    }

    private void SetupAnimations()
    {
        _animator = AddComponent<Animator>();

        // Walk animation
        var walkFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Warrior_walk_1"),
            ResourceManager.Instance.GetTexture("Warrior_walk_2")
        };
        var walkAnimation = new Animation(walkFrames, 0.2f);
        _animator.AddAnimation("walk", walkAnimation);

        // Attack animation
        var attackFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Warrior_attack_1"),
            ResourceManager.Instance.GetTexture("Warrior_attack_2"),
            ResourceManager.Instance.GetTexture("Warrior_attack_3"),
            ResourceManager.Instance.GetTexture("Warrior_attack_4")
        };
        var attackAnimation = new Animation(attackFrames, 0.2f);
        _animator.AddAnimation("attack", attackAnimation);

        _animator.Play("walk");
        if (walkFrames[0] != null)
        {
            _renderer.Origin = new Vector2(walkFrames[0].Width / 2f, walkFrames[0].Height / 2f);
        }
    }

    private void Move(float deltaTime) {
        Vector2 direction = Vector2.Normalize(GameConstants.CENTER - Position);
        Position += direction * _moveSpeed * deltaTime;
    }

    private void FindTarget() {
        // Check for Ally in range
        Ally nearestAlly = FindNearestAlly();
        if (nearestAlly != null && Vector2.Distance(Position, nearestAlly.Position) < 30f) {
            currentTarget = nearestAlly;
            state = EnemyState.Attacking;
            return;
        }
        
        // If no Ally, target Castle
        if (Vector2.Distance(Position, GameConstants.CENTER) < 80f) {
            currentTarget = GameScene.Castle;
            state = EnemyState.Attacking;
        }
    }

    private Ally FindNearestAlly() {
        Ally nearestAlly = null;
        float nearestDistance = float.MaxValue;

        if (_parentLane == null)
            return null;

        foreach (var ally in _parentLane.Allies) {
            if (ally.Active) {
                float distance = Vector2.Distance(Position, ally.Position);
                if (distance < nearestDistance) {
                    nearestDistance = distance;
                    nearestAlly = ally;
                }
            }
        }

        return nearestAlly;
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    private void StartAttack()
    {
        if (_isAttacking) return;

        _isAttacking = true;
        _attackTimer = ATTACK_DURATION;
        _animator.Play("attack");
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

    private void UpdateAnimation()
    {
        var currentAnimation = _animator.GetCurrentAnimation();
        if (currentAnimation?.CurrentFrame != null)
        {
            _renderer.Texture = currentAnimation.CurrentFrame;
        }
    }

    public override void Update(GameTime gameTime) {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        switch (state) {
            case EnemyState.Moving:
                Move(deltaTime);
                FindTarget();
                break;
                
            case EnemyState.Attacking:
                if (currentTarget == null ||
                    currentTarget.GetComponent<HealthComponent>()?.IsDead() == true)
                {
                    state = EnemyState.Moving;
                    currentTarget = null;
                    _animator.Play("walk");
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

        UpdateAnimation();

        base.Update(gameTime);
    }
}