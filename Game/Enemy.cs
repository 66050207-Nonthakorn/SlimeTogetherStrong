using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public abstract class Enemy : GameObject {
    // Configurable in derived enemy classes
    protected GameObject _enemyClass = null;
    protected float _moveSpeed;
    protected int _maxHp;
    protected int _damage;
    protected float _attackSpeed;
    protected float _attackRange;
    protected float _colliderRadius;

    // Enemy state management
    private EnemyState state = EnemyState.Moving;
    private GameObject currentTarget;

    private bool _isAttacking { get; set; } = false;
    private float _attackTimer = 0f;
    private const float ATTACK_DURATION = 1f;

    // Animation
    protected Animator _animator;
    protected SpriteRenderer _renderer;

    // Reference to scene and lane
    private GameScene _scene;
    private LaneData _parentLane;
    private int _slotIndex;

    protected Enemy()
    {
        Tag = "Enemy";
        Scale = new Vector2(0.2f, 0.2f);
    }

    protected void SetupEnemy()
    {
        SetupCollider();
        SetupHealth();
        SetupCombat();
        SetupHealthBar();
    }

    protected void SetupRenderer()
    {
        _renderer = _enemyClass.AddComponent<SpriteRenderer>();
    }

    protected abstract void SetupAnimations();

    private void SetupCollider() {
        var collider = _enemyClass.AddComponent<CircleCollider>();
        collider.Radius = _colliderRadius;
    }
    
    private void SetupHealth() {
        var health = _enemyClass.AddComponent<HealthComponent>();
        health.MaxHP = _maxHp;
        health.Initialize();

        health.OnDamage += (damage) => {};

        health.OnDeath += () =>
        {
            System.Diagnostics.Debug.WriteLine($"Killed enemy");
            Active = false;

            for (int i = 0; i < _parentLane.Enemies.Count; i++)
            {
                _parentLane.Enemies[i].SetSlotIndex(i);
            }
        };
    }

    private void SetupCombat() {
        var combat = _enemyClass.AddComponent<CombatComponent>();
        combat.Damage = _damage;
        combat.AttackSpeed = _attackSpeed;
        combat.AttackRange = _attackRange;

        combat.OnAttack += (target) =>
        {
            _animator.Play("attack");
        };
    }

    private void SetupHealthBar()
    {
        var healthBar = _enemyClass.AddComponent<HealthBar>();
        healthBar.Size = new Vector2(50, 6);
        healthBar.Offset = new Vector2(0, -40);
        healthBar.FillColor = Color.Red;
        healthBar.BackgroundColor = new Color(60, 60, 60);
        healthBar.BorderColor = Color.Black;
        healthBar.BorderThickness = 1;
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