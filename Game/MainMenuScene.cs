using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public class MainMenuScene : Scene
{
    public override void Load()
    {
        // Get screen dimensions from SceneManager
        int screenWidth = SceneManager.Instance.ScreenWidth;
        int screenHeight = SceneManager.Instance.ScreenHeight;

        // Background
        var background = new GameObject
        {
            Position = Vector2.Zero
        };
        var bgSprite = background.AddComponent<SpriteRenderer>();
        bgSprite.Texture = ResourceManager.Instance.GetTexture("Main_Menu");
        
        // Calculate scale to fit the screen
        if (bgSprite.Texture != null)
        {
            float scaleX = (float)screenWidth / bgSprite.Texture.Width;
            float scaleY = (float)screenHeight / bgSprite.Texture.Height;
            background.Scale = new Vector2(scaleX, scaleY);
        }
        
        AddGameObject(background);

        // Button dimensions
        float buttonWidth = 320;
        float buttonHeight = 80;
        float buttonSpacing = 20;

        // Center alignment - calculate the TOP-LEFT position of buttons
        float startX = (screenWidth - buttonWidth) / 2;
        float startY = 540;

        // Start Game Button
        var startButton = new TextButton(
            "Start Game",
            new Vector2(startX, startY),
            new Vector2(buttonWidth, buttonHeight),
            OnStartGameClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(startButton);

        // Settings Button
        var settingsButton = new TextButton(
            "Settings",
            new Vector2(startX, startY + buttonHeight + buttonSpacing),
            new Vector2(buttonWidth, buttonHeight),
            OnSettingsClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(settingsButton);

        // Quit Game Button
        var quitButton = new TextButton(
            "Quit Game",
            new Vector2(startX, startY + (buttonHeight + buttonSpacing) * 2),
            new Vector2(buttonWidth, buttonHeight),
            OnQuitGameClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(quitButton);

        base.Load();
    }

    private void OnStartGameClick()
    {
        // Load the game scene
        SceneManager.Instance.LoadScene("GameScene");
    }

    private void OnSettingsClick()
    {
        // Open settings as an overlay
        SceneManager.Instance.PushOverlay(new SettingsScene());
    }

    private void OnQuitGameClick()
    {
        // Exit the game
        System.Environment.Exit(0);
    }
}
