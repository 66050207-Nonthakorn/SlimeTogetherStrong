using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class LaneData
{
    public int Index;
    public float Angle;

    public List<Ally> Allies;

    public const int MAX_ALLIES = 4;

    public LaneData(int index, float angle)
    {
        Index = index;
        Angle = angle;
        Allies = new List<Ally>();
    }

    public bool CanAddAlly()
    {
        return Allies.Count < MAX_ALLIES;
    }

    public void AddAlly(Ally ally)
    {
        if (!CanAddAlly()) return;

        Allies.Add(ally);
        ally.Initialize(this, Allies.Count - 1);
    }

    public void RemoveAlly(Ally ally)
    {
        Allies.Remove(ally);
    }

    public Vector2 GetFormationPosition(int slotIndex)
    {
        float spacing = 30f;
        float offset = (slotIndex - (MAX_ALLIES - 1) / 2f) * spacing;

        Vector2 dir = new Vector2(
            MathF.Cos(Angle),
            MathF.Sin(Angle)
        );

        Vector2 normal = new Vector2(-dir.Y, dir.X);

        return normal * offset;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var ally in Allies)
            ally.Update(gameTime);
    }
}