using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public class PauseScene : Scene
{
    public override void Load()
    {
        // Get screen dimensions from SceneManager
        int screenWidth = SceneManager.Instance.ScreenWidth;
        int screenHeight = SceneManager.Instance.ScreenHeight;

        // Semi-transparent dark overlay background (covers entire screen)
        var overlayBg = new GameObject
        {
            Position = Vector2.Zero
        };
        var overlayRenderer = overlayBg.AddComponent<SpriteRenderer>();
        AddGameObject(overlayBg);

        // Panel dimensions
        float panelWidth = 500;
        float panelHeight = 450;
        float panelX = (screenWidth - panelWidth) / 2;
        float panelY = (screenHeight - panelHeight) / 2;

        // Pause panel background
        var panelBg = new GameObject
        {
            Position = new Vector2(panelX, panelY)
        };
        var panelRenderer = panelBg.AddComponent<RoundedRectangleRenderer>();
        panelRenderer.Size = new Vector2(panelWidth, panelHeight);
        panelRenderer.FillColor = new Color(40, 40, 40, 240);
        panelRenderer.OutlineColor = Color.White;
        panelRenderer.OutlineThickness = 3;
        panelRenderer.CornerRadius = 20;
        AddGameObject(panelBg);

        // Pause title
        var titleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, panelY + 60),
            Scale = new Vector2(2, 2)
        };
        var title = titleObj.AddComponent<Text>();
        title.Font = ResourceManager.Instance.GetFont("DefaultFont");
        title.Content = "Paused";
        title.Color = Color.White;
        // Use unscaled measurement for origin
        var titleSize = title.Font.MeasureString(title.Content);
        title.Origin = titleSize / 2;
        AddGameObject(titleObj);

        // Button dimensions
        float buttonWidth = 320;
        float buttonHeight = 70;
        float buttonSpacing = 20;

        // Center alignment
        float startX = (screenWidth - buttonWidth) / 2;
        float startY = panelY + 150;

        // Resume Button
        var resumeButton = new TextButton(
            "Resume Game",
            new Vector2(startX, startY),
            new Vector2(buttonWidth, buttonHeight),
            OnResumeClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(resumeButton);

        // Settings Button
        var settingsButton = new TextButton(
            "Settings",
            new Vector2(startX, startY + buttonHeight + buttonSpacing),
            new Vector2(buttonWidth, buttonHeight),
            OnSettingsClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(settingsButton);

        // Return to Main Menu Button
        var mainMenuButton = new TextButton(
            "Return to Main Menu",
            new Vector2(startX, startY + (buttonHeight + buttonSpacing) * 2),
            new Vector2(buttonWidth, buttonHeight),
            OnMainMenuClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(mainMenuButton);

        base.Load();
    }

    public override void Update(GameTime gameTime)
    {
        // Check for ESC key to close pause menu
        if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            SceneManager.Instance.PopOverlay();
            return; // Don't process rest of update when closing
        }

        base.Update(gameTime);
    }

    private void OnResumeClick()
    {
        // Close the pause overlay
        SceneManager.Instance.PopOverlay();
    }

    private void OnSettingsClick()
    {
        // Open settings as another overlay on top of pause menu
        SceneManager.Instance.PushOverlay(new SettingsScene());
    }

    private void OnMainMenuClick()
    {
        // Return to main menu (this will clear all overlays)
        SceneManager.Instance.LoadScene("MainMenu");
    }
}
