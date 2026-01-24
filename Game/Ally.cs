using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine;

public class Ally : GameObject
{
    public int SlotIndex;
    public LaneData ParentLane;
    public Vector2 FormationOffset;

    public int MaxHP = 100;
    public int CurrentHP;

    public void Initialize(LaneData lane, int slotIndex)
    {
        ParentLane = lane;
        SlotIndex = slotIndex;
        FormationOffset = lane.GetFormationPosition(slotIndex);

        CurrentHP = MaxHP;
    }

    public override void Update(GameTime gameTime)
    {
        if (ParentLane == null || IsDead())
            return;

        Vector2 basePos =
            MapManager.Instance.GetPositionOnRing(
                RingType.Blue_Defense,
                ParentLane.Angle
            );

        Position = basePos + FormationOffset;

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
        ParentLane?.RemoveAlly(this);
    }

    private void FindTarget()
    {
        // TODO:
        // 1. loop enemy list
        // 2. เช็ค angle ใกล้ ParentLane.Angle
        // 3. เช็ค distance < AttackRange
    }
}