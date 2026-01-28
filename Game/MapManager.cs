using System;
using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Game;

public class MapManager
{
    public const int LANE_COUNT = 6;

    public Vector2 CenterPosition = GameConstants.CENTER;
    public LaneData[] Lanes;
    //public Castle Castle;
    public static MapManager Instance { get; private set; }

    public MapManager()
    {
        Instance = this;

        //Castle = new Castle(CenterPosition, 100);
        InitializeLanes();
    }

    private void InitializeLanes()
    {
        Lanes = new LaneData[LANE_COUNT];

        float spawnDistance = GameConstants.ENEMY_SPAWN_RADIUS; 

        for (int i = 0; i < LANE_COUNT; i++)
        {
            float angle = MathHelper.TwoPi / LANE_COUNT * i;

            Vector2 dir = new Vector2(
                MathF.Cos(angle),
                MathF.Sin(angle)
            );

            Vector2 startPoint = CenterPosition + dir * spawnDistance;
            Vector2 endPoint = CenterPosition; 

            Lanes[i] = new LaneData(i, startPoint, endPoint);
        }
    }

    public LaneData GetLaneByIndex(int index)
    {
        return Lanes[index];
    }

    public void Update(GameTime gameTime)
    {
        foreach (var lane in Lanes)
            lane.Update(gameTime);
    }

}
