using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class LaneData
{
    public const int MAX_ALLIES = 4;

    public int Index;

    // จุดเริ่มเลน (spawn enemy / ปลายถนน)
    public Vector2 StartPoint;

    // จุดปลายเลน (Castle)
    public Vector2 EndPoint;

    // ทิศทางของเลน (enemy เดินตาม)
    public Vector2 Direction;

    // เวกเตอร์ตั้งฉาก ใช้จัดฟันปลา
    public Vector2 Perpendicular;

    public List<Ally> Allies;

    public LaneData(int index, Vector2 startPoint, Vector2 endPoint)
    {
        Index = index;
        StartPoint = startPoint;
        EndPoint = endPoint;

        Allies = new List<Ally>();

        Direction = Vector2.Normalize(StartPoint - EndPoint);

        // หมุน 90 องศา
        Perpendicular = new Vector2(-Direction.Y, Direction.X);

        Vector2 fromCastle = StartPoint - EndPoint;
    }

    public bool CanAddAlly()
    {
        return Allies.Count < MAX_ALLIES;
    }

    public void AddAlly(Ally ally)
    {
        if (!CanAddAlly())
            return;

        Allies.Add(ally);
        ally.Initialize(this, Allies.Count - 1);
    }

    public void RemoveAlly(Ally ally)
    {
        if (!Allies.Remove(ally))
            return;

        // จัด SlotIndex ใหม่ให้ Ally ที่เหลือ
        for (int i = 0; i < Allies.Count; i++)
        {
            Allies[i].SlotIndex = i;
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var ally in Allies)
        {
            if (ally.Active)
                ally.Update(gameTime);
        }
    }

    internal int GetNextAllySlotIndex()
    {
        throw new NotImplementedException();
    }
}
