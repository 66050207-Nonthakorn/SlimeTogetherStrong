using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;
using SlimeTogetherStrong.Game;

namespace SlimeTogetherStrong;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _backgroundTexture;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1000;
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.ApplyChanges();

        Window.Title = "Slime Together Strong";

        // Set screen dimensions in SceneManager
        SceneManager.Instance.ScreenWidth = _graphics.PreferredBackBufferWidth;
        SceneManager.Instance.ScreenHeight = _graphics.PreferredBackBufferHeight;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Should load resources before starting the scene
        ResourceManager.Instance.LoadFont("DefaultFont", Content.Load<SpriteFont>("font"));

        // Textures - UI
        ResourceManager.Instance.LoadTexture("Main_Menu", Content.Load<Texture2D>("UI/Main_Menu"));
        ResourceManager.Instance.LoadTexture("Skill_Icon_Active", Content.Load<Texture2D>("UI/Skill_Icon_Active"));
        ResourceManager.Instance.LoadTexture("Skill_Icon_Inactive", Content.Load<Texture2D>("UI/Skill_Icon_ Unactive"));
        ResourceManager.Instance.LoadTexture("Health_Icon", Content.Load<Texture2D>("UI/Health"));
        ResourceManager.Instance.LoadTexture("Mana_Icon", Content.Load<Texture2D>("UI/Mana_point_"));

        // Textures - Main
        _backgroundTexture = Content.Load<Texture2D>("Main/Background");
        ResourceManager.Instance.LoadTexture("lane_road", Content.Load<Texture2D>("Main/Allies_Street"));

        // Textures - Castle Idle (5 frames)
        ResourceManager.Instance.LoadTexture("Castle_idle_0", Content.Load<Texture2D>("Castle_idle/Base-1"));
        ResourceManager.Instance.LoadTexture("Castle_idle_1", Content.Load<Texture2D>("Castle_idle/Base-2"));
        ResourceManager.Instance.LoadTexture("Castle_idle_2", Content.Load<Texture2D>("Castle_idle/Base-3"));
        ResourceManager.Instance.LoadTexture("Castle_idle_3", Content.Load<Texture2D>("Castle_idle/Base-4"));
        ResourceManager.Instance.LoadTexture("Castle_idle_4", Content.Load<Texture2D>("Castle_idle/Base-5"));

        // Textures - Player Idle (5 frames)
        ResourceManager.Instance.LoadTexture("P_idle_0", Content.Load<Texture2D>("Player_idle/P_idle_0"));
        ResourceManager.Instance.LoadTexture("P_idle_1", Content.Load<Texture2D>("Player_idle/P_idle_1"));
        ResourceManager.Instance.LoadTexture("P_idle_2", Content.Load<Texture2D>("Player_idle/P_idle_2"));
        ResourceManager.Instance.LoadTexture("P_idle_3", Content.Load<Texture2D>("Player_idle/P_idle_3"));
        ResourceManager.Instance.LoadTexture("P_idle_4", Content.Load<Texture2D>("Player_idle/P_idle_4"));

        // Textures - Player Attack (4 frames)
        ResourceManager.Instance.LoadTexture("P_attack_1", Content.Load<Texture2D>("Player_attack/P_attack_1"));
        ResourceManager.Instance.LoadTexture("P_attack_2", Content.Load<Texture2D>("Player_attack/P_attack_2"));
        ResourceManager.Instance.LoadTexture("P_attack_3", Content.Load<Texture2D>("Player_attack/P_attack_3"));
        ResourceManager.Instance.LoadTexture("P_attack_4", Content.Load<Texture2D>("Player_attack/P_attack_4"));

        // Textures - Ally Idle (4 frames)
        ResourceManager.Instance.LoadTexture("A_idle_0", Content.Load<Texture2D>("Ally_idle/Slime_Friend_Idle-1"));
        ResourceManager.Instance.LoadTexture("A_idle_1", Content.Load<Texture2D>("Ally_idle/Slime_Friend_Idle-2"));
        ResourceManager.Instance.LoadTexture("A_idle_2", Content.Load<Texture2D>("Ally_idle/Slime_Friend_Idle-3"));
        ResourceManager.Instance.LoadTexture("A_idle_3", Content.Load<Texture2D>("Ally_idle/Slime_Friend_Idle-4"));

        // Textures - Ally Attack (5 frames)
        ResourceManager.Instance.LoadTexture("A_attack_1", Content.Load<Texture2D>("Ally_Attack/Slime_Friend_Attack-1"));
        ResourceManager.Instance.LoadTexture("A_attack_2", Content.Load<Texture2D>("Ally_Attack/Slime_Friend_Attack-2"));
        ResourceManager.Instance.LoadTexture("A_attack_3", Content.Load<Texture2D>("Ally_Attack/Slime_Friend_Attack-3"));
        ResourceManager.Instance.LoadTexture("A_attack_4", Content.Load<Texture2D>("Ally_Attack/Slime_Friend_Attack-4"));
        ResourceManager.Instance.LoadTexture("A_attack_5", Content.Load<Texture2D>("Ally_Attack/Slime_Friend_Attack-5"));
        // Textures - Fireball Attack (2 frames)
        ResourceManager.Instance.LoadTexture("Fireball/attack/Ranged_Attack-1", Content.Load<Texture2D>("Fireball/attack/Ranged_Attack-1"));
        ResourceManager.Instance.LoadTexture("Fireball/attack/Ranged_Attack-2", Content.Load<Texture2D>("Fireball/attack/Ranged_Attack-2"));

        // Textures - Fireball Explosion (6 frames)
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-1", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-1"));
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-2", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-2"));
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-3", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-3"));
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-4", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-4"));
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-5", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-5"));
        ResourceManager.Instance.LoadTexture("Fireball/explosion/Fireball_Explosion-6", Content.Load<Texture2D>("Fireball/explosion/Fireball_Explosion-6"));

        // Textures - Enemy: Warrior class walk (2 frames)
        ResourceManager.Instance.LoadTexture("Warrior_walk_1", Content.Load<Texture2D>("Enemies/Warrior_walk/Warrior_walk_1"));
        ResourceManager.Instance.LoadTexture("Warrior_walk_2", Content.Load<Texture2D>("Enemies/Warrior_walk/Warrior_walk_2"));

        // Textures - Enemy: Warrior class attack (4 frames)
        ResourceManager.Instance.LoadTexture("Warrior_attack_1", Content.Load<Texture2D>("Enemies/Warrior_attack/Warrior_attack_1"));
        ResourceManager.Instance.LoadTexture("Warrior_attack_2", Content.Load<Texture2D>("Enemies/Warrior_attack/Warrior_attack_2"));
        ResourceManager.Instance.LoadTexture("Warrior_attack_3", Content.Load<Texture2D>("Enemies/Warrior_attack/Warrior_attack_3"));
        ResourceManager.Instance.LoadTexture("Warrior_attack_4", Content.Load<Texture2D>("Enemies/Warrior_attack/Warrior_attack_4"));

        // Textures - Enemy: Ninja class walk (2 frames)
        ResourceManager.Instance.LoadTexture("Ninja_walk_1", Content.Load<Texture2D>("Enemies/Ninja_walk/Ninja_walk_1"));
        ResourceManager.Instance.LoadTexture("Ninja_walk_2", Content.Load<Texture2D>("Enemies/Ninja_walk/Ninja_walk_2"));

        // Textures - Enemy: Ninja class attack (4 frames)
        ResourceManager.Instance.LoadTexture("Ninja_attack_1", Content.Load<Texture2D>("Enemies/Ninja_attack/Ninja_attack_1"));
        ResourceManager.Instance.LoadTexture("Ninja_attack_2", Content.Load<Texture2D>("Enemies/Ninja_attack/Ninja_attack_2"));
        ResourceManager.Instance.LoadTexture("Ninja_attack_3", Content.Load<Texture2D>("Enemies/Ninja_attack/Ninja_attack_3"));
        ResourceManager.Instance.LoadTexture("Ninja_attack_4", Content.Load<Texture2D>("Enemies/Ninja_attack/Ninja_attack_4"));

        // Textures - Enemy: Spearsman class walk (2 frames)
        ResourceManager.Instance.LoadTexture("Spearsman_walk_1", Content.Load<Texture2D>("Enemies/Spearsman_walk/Spearsman_walk_1"));
        ResourceManager.Instance.LoadTexture("Spearsman_walk_2", Content.Load<Texture2D>("Enemies/Spearsman_walk/Spearsman_walk_2"));

        // Textures - Enemy: Spearsman class attack (4 frames)
        ResourceManager.Instance.LoadTexture("Spearsman_attack_1", Content.Load<Texture2D>("Enemies/Spearsman_attack/Spearsman_attack_1"));
        ResourceManager.Instance.LoadTexture("Spearsman_attack_2", Content.Load<Texture2D>("Enemies/Spearsman_attack/Spearsman_attack_2"));
        ResourceManager.Instance.LoadTexture("Spearsman_attack_3", Content.Load<Texture2D>("Enemies/Spearsman_attack/Spearsman_attack_3"));
        ResourceManager.Instance.LoadTexture("Spearsman_attack_4", Content.Load<Texture2D>("Enemies/Spearsman_attack/Spearsman_attack_4"));

        // Textures - Tank class walk (2 frames)
        ResourceManager.Instance.LoadTexture("Tank_walk_1", Content.Load<Texture2D>("Enemies/Tank_walk/Tank_walk_1"));
        ResourceManager.Instance.LoadTexture("Tank_walk_2", Content.Load<Texture2D>("Enemies/Tank_walk/Tank_walk_2"));

        // Textures - Tank class attack (5 frames)
        ResourceManager.Instance.LoadTexture("Tank_attack_1", Content.Load<Texture2D>("Enemies/Tank_attack/Tank_attack_1"));
        ResourceManager.Instance.LoadTexture("Tank_attack_2", Content.Load<Texture2D>("Enemies/Tank_attack/Tank_attack_2"));
        ResourceManager.Instance.LoadTexture("Tank_attack_3", Content.Load<Texture2D>("Enemies/Tank_attack/Tank_attack_3"));
        ResourceManager.Instance.LoadTexture("Tank_attack_4", Content.Load<Texture2D>("Enemies/Tank_attack/Tank_attack_4"));
        ResourceManager.Instance.LoadTexture("Tank_attack_5", Content.Load<Texture2D>("Enemies/Tank_attack/Tank_attack_5"));

        // สร้าง Scene หลังจากโหลด texture แล้ว
        SceneManager.Instance.AddScene("MainMenu", new MainMenuScene());

        // Start the initial scene
        SceneManager.Instance.LoadScene("MainMenu");
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update();
        Button.ResetClickFlag();
        SceneManager.Instance.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // วาด background
        _spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);

        // วาด lane roads (6 เส้นถนน ทุกๆ 60°)
        var currentScene = SceneManager.Instance.GetCurrentScene();
        if (currentScene is SlimeTogetherStrong.Game.GameScene gameScene)
        {
            DrawLaneRoads(_spriteBatch);
            gameScene.DrawPlacementHighlights(_spriteBatch, GraphicsDevice);
        }

        SceneManager.Instance.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawLaneRoads(SpriteBatch spriteBatch)
    {
        var laneTexture = ResourceManager.Instance.GetTexture("lane_road");
        if (laneTexture == null) return;

        int laneCount = 6;
        float angleStep = MathHelper.TwoPi / laneCount;
        Vector2 center = new Vector2(500, 500); // GameConstants.CENTER

        for (int i = 0; i < laneCount; i++)
        {
            float angle = i * angleStep;

            // Origin ที่ด้านล่างกลางของ texture (จุดที่ติดกับ center)
            Vector2 origin = new Vector2(laneTexture.Width / 2f, laneTexture.Height);

            spriteBatch.Draw(
                laneTexture,
                center,
                null,
                Color.White,
                angle - MathHelper.PiOver2, // หมุนให้ชี้ออกจาก center
                origin,
                1f,
                SpriteEffects.None,
                0
            );
        }
    }

}
