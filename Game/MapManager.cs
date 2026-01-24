using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MapManager
{
    public const int LANE_COUNT = 9;

    public const float ORANGE_RADIUS = 150f;
    public const float BLUE_RADIUS = 250f;
    public const float GREEN_RADIUS = 350f;

    public Vector2 CenterPosition;
    public LaneData[] Lanes;
    public Castle Castle;

    public static MapManager Instance { get; private set; }

    public MapManager(Vector2 center)
    {
        Instance = this;
        CenterPosition = center;

        InitializeLanes();
        Castle = new Castle(center, 100);
    }

    private void InitializeLanes()
    {
        Lanes = new LaneData[LANE_COUNT];

        for (int i = 0; i < LANE_COUNT; i++)
        {
            float angle = MathHelper.TwoPi / LANE_COUNT * i;
            Lanes[i] = new LaneData(i, angle);
        }
    }

    public LaneData GetLaneByIndex(int index)
    {
        return Lanes[index];
    }

    public LaneData GetLaneByAngle(float angle)
    {
        angle = MathHelper.WrapAngle(angle);

        int index = (int)MathF.Round(angle / MathHelper.TwoPi * LANE_COUNT) % LANE_COUNT;

        return Lanes[index];
    }

    public Vector2 GetPositionOnRing(RingType ring, float angle)
    {
        float radius = ring switch
        {
            RingType.Orange_Player => ORANGE_RADIUS,
            RingType.Blue_Defense => BLUE_RADIUS,
            RingType.Green_Engagement => GREEN_RADIUS,
            _ => ORANGE_RADIUS
        };

        return CenterPosition + new Vector2(
            MathF.Cos(angle),
            MathF.Sin(angle)
        ) * radius;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var lane in Lanes)
            lane.Update(gameTime);
    }

    public void DrawDebug(SpriteBatch spriteBatch)
    {
        
    }
}