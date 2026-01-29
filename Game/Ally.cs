using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Game;

public class Ally : GameObject
{
    public int SlotIndex;
    public LaneData ParentLane;

    public int MaxHP = 100;
    public int CurrentHP;

    // public Enemy CurrentTarget;

    public void Initialize(LaneData lane, int slotIndex)
    {
        ParentLane = lane;
        SlotIndex = slotIndex;
        CurrentHP = MaxHP;
    }

    public override void Update(GameTime gameTime)
    {
        if (ParentLane == null || IsDead())
            return;

        float forwardOffset = GameConstants.BLUE_RADIUS + SlotIndex * GameConstants.FORWARD_SPACING;

        float sideOffset = (SlotIndex % 2 == 0 ? -1 : 1) * GameConstants.SIDE_SPACING;

        Position =
            ParentLane.EndPoint
            + ParentLane.Direction * forwardOffset
            + ParentLane.Perpendicular * sideOffset;

        FindTarget();
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
            CurrentHP = 0;

        if (IsDead())
            OnDeath();
    }

    public bool IsDead()
    {
        return CurrentHP <= 0;
    }

    private void OnDeath()
    {
        Active = false;
        ParentLane?.RemoveAlly(this);
    }

    private void FindTarget()
    {
    //     CurrentTarget = null;
    //     float closestDistance = float.MaxValue;

    //     foreach (var enemy in GameScene.Instance.Enemies)
    //     {
    //         if (!enemy.Active)
    //             continue;

    //         // 1. ต้องอยู่เลนเดียวกัน
    //         if (enemy.ParentLane != ParentLane)
    //             continue;

    //         // 2. เช็คระยะ
    //         float distance =
    //             Vector2.Distance(enemy.Position, Position);

    //         if (distance > AttackRange)
    //             continue;

    //         // 3. เลือกตัวที่ใกล้ Castle ที่สุด
    //         float distanceToCastle =
    //             Vector2.Distance(enemy.Position, ParentLane.EndPoint);

    //         if (distanceToCastle < closestDistance)
    //         {
    //             closestDistance = distanceToCastle;
    //             CurrentTarget = enemy;
    //         }
    //     }
    }
}
