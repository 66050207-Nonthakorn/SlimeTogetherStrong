using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Game.Entities;
using System;
using System.Collections.Generic;

namespace SlimeTogetherStrong.Game.Systems;

public class LaneData
{
    public const int MAX_ALLIES = 4;

    public int Index;

    public Vector2 StartPoint;

    public Vector2 EndPoint;

    public Vector2 Direction;

    public Vector2 Perpendicular;

    public List<Ally> Allies;
    public List<Enemy> Enemies;

    public LaneData(int index, Vector2 startPoint, Vector2 endPoint)
    {
        Index = index;
        StartPoint = startPoint;
        EndPoint = endPoint;

        Allies = new List<Ally>();
        Enemies = new List<Enemy>();

        Direction = Vector2.Normalize(StartPoint - EndPoint);

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
        ally.SetupLane(this, Allies.Count - 1);
    }

    public void AddEnemy(Enemy enemy)
    {
        if (!CanAddAlly())
            return;

        Enemies.Add(enemy);
        enemy.SetParentLane(this, Enemies.Count - 1);
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
