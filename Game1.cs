using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Game;

namespace SlimeTogetherStrong;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;


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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Should load resources before starting the scene
        ResourceManager.Instance.LoadFont("DefaultFont", Content.Load<SpriteFont>("font"));

        // Textures - Main
        ResourceManager.Instance.LoadTexture("castle", Content.Load<Texture2D>("Main/castle"));
        ResourceManager.Instance.LoadTexture("player_lane", Content.Load<Texture2D>("Main/player_lane"));
        ResourceManager.Instance.LoadTexture("allies_lane", Content.Load<Texture2D>("Main/allies_lane"));
        ResourceManager.Instance.LoadTexture("bullet", Content.Load<Texture2D>("Main/bullet"));

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


        // สร้าง Scene หลังจากโหลด texture แล้ว
        SceneManager.Instance.AddScene("GameScene", new GameScene());

        // Start the initial scene
        SceneManager.Instance.LoadScene("GameScene");
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update();
        SceneManager.Instance.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(0x90, 0xEE, 0x90));

        _spriteBatch.Begin();

        // วาด debug rings (optional)
        var currentScene = SceneManager.Instance.GetCurrentScene();
        if (currentScene is SlimeTogetherStrong.Game.GameScene gameScene)
        {
            gameScene.DrawDebugRings(_spriteBatch, GraphicsDevice);
        }

        SceneManager.Instance.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

}
