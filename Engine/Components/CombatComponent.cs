using System;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components;

public class CombatComponent : Component
{
    public int Damage { get; set; } = 10;
    public float AttackSpeed { get; set; } = 1f;      // attacks per second
    public float AttackRange { get; set; } = 50f;     // pixels

    private float _attackCooldown = 0f;

    // Events
    public event Action<GameObject> OnAttack;  // parameter = target

    public override void Update(GameTime gameTime)
    {
        if (!Enabled) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Reduce cooldown
        if (_attackCooldown > 0)
        {
            _attackCooldown -= deltaTime;
        }
    }

    public bool CanAttack()
    {
        return Enabled && _attackCooldown <= 0;
    }

    public void Attack(GameObject target)
    {
        if (!CanAttack()) return;
        if (target == null) return;

        // Reset cooldown
        _attackCooldown = 1f / AttackSpeed;

        // Deal damage if target has HealthComponent
        var targetHealth = target.GetComponent<HealthComponent>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(Damage);
        }

        // Fire event
        OnAttack?.Invoke(target);
    }

    public bool IsInRange(GameObject target)
    {
        if (target == null || GameObject == null) return false;

        float distance = Vector2.Distance(GameObject.Position, target.Position);
        return distance <= AttackRange;
    }

    public void ResetCooldown()
    {
        _attackCooldown = 0f;
    }

    public float GetCooldownPercentage()
    {
        if (AttackSpeed <= 0) return 0f;
        float maxCooldown = 1f / AttackSpeed;
        return Math.Clamp(_attackCooldown / maxCooldown, 0f, 1f);
    }

    // === Getters สำหรับ P3 (Enemy) ===

    /// <summary>
    /// เวลา cooldown ที่เหลือ (วินาที)
    /// </summary>
    public float GetRemainingCooldown()
    {
        return Math.Max(0f, _attackCooldown);
    }

    /// <summary>
    /// กำลังอยู่ใน cooldown หรือไม่
    /// </summary>
    public bool IsOnCooldown => _attackCooldown > 0;
}
