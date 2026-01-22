using Microsoft.Xna.Framework;

  namespace SlimeTogetherStrong.Game;

  public static class GameConstants
  {
      // Screen
      public const int SCREEN_WIDTH = 1000;
      public const int SCREEN_HEIGHT = 1000;

      // Castle position - Center Screen
      public static readonly Vector2 CENTER = new Vector2(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f);

      // Ring radiuses
      public const float ORANGE_RADIUS = 150f;  // Player ring
      public const float BLUE_RADIUS = 250f;    // Allies
      public const float GREEN_RADIUS = 350f;   // Start attack enemies 

      // Lanes
      public const int LANE_COUNT = 9;
      public const float LANE_ANGLE_STEP = 360f / LANE_COUNT;  // 40 degrees per lane
  }