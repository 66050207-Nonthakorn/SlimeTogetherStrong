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

    // Animation
    private Animator _animator;
    private SpriteRenderer _renderer;

    // Reference to scene
    private GameScene _scene;

    public Enemy()
    {
        Tag = "Enemy";
        Scale = new Vector2(0.2f, 0.2f);

        SetupComponentsAndListeners();
        SetupRenderer();
        SetupAnimations();
    }

    private void SetupComponentsAndListeners()
    {
        var collider = AddComponent<CircleCollider>();
        collider.Radius = 30f;

        var health = AddComponent<HealthComponent>();
        health.MaxHP = 50;
        health.Initialize();

        health.OnDamage += (damage) =>
        {
            System.Diagnostics.Debug.WriteLine($"Enemy HP: {health.CurrentHP}/{health.MaxHP}");
        };

        health.OnDeath += () =>
        {
            System.Diagnostics.Debug.WriteLine($"Enemy killed.");
            Active = false;
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
        // Ally nearestAlly = FindNearestAlly();
        // if (nearestAlly != null && Vector2.Distance(Position, nearestAlly.Position) < 30f) {
        //     currentTarget = nearestAlly;
        //     state = EnemyState.Attacking;
        //     return;
        // }
        
        // If no Ally, target Castle
        if (Vector2.Distance(Position, GameConstants.CENTER) < 50f) {
            currentTarget = GameScene.Castle;
            state = EnemyState.Attacking;
        }
    }

    // private Ally FindNearestAlly() {
    //     Ally nearestAlly = null;
    //     float nearestDistance = float.MaxValue;

    //     foreach (var obj in MapManager.GameObjects) {
    //         if (obj is Ally ally) {
    //             float distance = Vector2.Distance(Position, ally.Position);
    //             if (distance < nearestDistance) {
    //                 nearestDistance = distance;
    //                 nearestAlly = ally;
    //             }
    //         }
    //     }

    //     return nearestAlly;
    // }

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
                if (currentTarget == null || currentTarget.GetComponent<HealthComponent>()?.IsDead() != false) {
                    state = EnemyState.Moving;
                    currentTarget = null;
                } else {
                    GetComponent<CombatComponent>().Attack(currentTarget);
                }
                break;
        }

        UpdateAnimation();

        base.Update(gameTime);
    }
}