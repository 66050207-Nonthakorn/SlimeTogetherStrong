using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        SceneManager.Instance.AddScene("GameScene", new GameScene());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Should load resources before starting the scene
        ResourceManager.Instance.LoadFont("DefaultFont", Content.Load<SpriteFont>("font"));

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
        GraphicsDevice.Clear(Color.WhiteSmoke);

        _spriteBatch.Begin();
        SceneManager.Instance.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
