using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class Ninja : Enemy
{
    // Enemy stat config
    protected const float MoveSpeed = 90f;
    protected const int MaxHP = 30;
    protected const int Damage = 8;
    protected const float AttackSpeed = 2f;      // attacks per second
    protected const float AttackRange = 40f;     // pixels
    protected const float ColliderRadius = 30f;

    public Ninja()
    {
        _enemyClass = this;
        _moveSpeed = MoveSpeed;
        _maxHp = MaxHP;
        _damage = Damage;
        _attackSpeed = AttackSpeed;
        _attackRange = AttackRange;
        _colliderRadius = ColliderRadius;

        SetupRenderer();
        SetupAnimations();
        SetupEnemy();
    }

    protected override void SetupAnimations()
    {
        _animator = AddComponent<Animator>();

        // Walk animation
        var walkFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Ninja_walk_1"),
            ResourceManager.Instance.GetTexture("Ninja_walk_2")
        };
        var walkAnimation = new Animation(walkFrames, 0.2f);
        _animator.AddAnimation("walk", walkAnimation);

        // Attack animation
        var attackFrames = new List<Texture2D>
        {
            ResourceManager.Instance.GetTexture("Ninja_attack_1"),
            ResourceManager.Instance.GetTexture("Ninja_attack_2"),
            ResourceManager.Instance.GetTexture("Ninja_attack_3"),
            ResourceManager.Instance.GetTexture("Ninja_attack_4")
        };
        var attackAnimation = new Animation(attackFrames, 0.2f);
        _animator.AddAnimation("attack", attackAnimation);

        _animator.Play("walk");
        if (walkFrames[0] != null)
        {
            _renderer.Origin = new Vector2(walkFrames[0].Width / 2f, walkFrames[0].Height / 2f);
        }
    }
}
