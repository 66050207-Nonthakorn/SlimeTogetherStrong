using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.UI;
using SlimeTogetherStrong.Engine.Managers;
using System;

namespace SlimeTogetherStrong.Game.Scenes;

public class GameOverScene : Scene
{
    private bool _isWin;
    private string _playTime;

    public GameOverScene(bool isWin, string playTime)
    {
        _isWin = isWin;
        _playTime = playTime;
    }

    private void OnPlayAgain()
    {
        SceneManager.Instance.PopOverlay();

        var newGameScene = new GameScene();
        SceneManager.Instance.AddScene("Game", newGameScene);
        SceneManager.Instance.LoadScene("Game");
    }

    private void OnMainMenu()
    {
        SceneManager.Instance.PopOverlay();
        SceneManager.Instance.LoadScene("MainMenu");
    }

    public override void Load()
    {
        base.Load();

        int screenWidth = SceneManager.Instance.ScreenWidth;
        int screenHeight = SceneManager.Instance.ScreenHeight;

        var overlayBg = new GameObject
        {
            Position = Vector2.Zero
        };
        var overlayRenderer = overlayBg.AddComponent<RoundedRectangleRenderer>();
        overlayRenderer.Size = new Vector2(screenWidth, screenHeight);
        overlayRenderer.FillColor = new Color(0, 0, 0, 180);
        overlayRenderer.OutlineThickness = 0;
        AddGameObject(overlayBg);

        float centerX = screenWidth / 2;
        float centerY = screenHeight / 2;

        var titleObj = new GameObject
        {
            Position = new Vector2(centerX, centerY - 150),
            Scale = new Vector2(2.0f, 2.0f)
        };
        var titleText = titleObj.AddComponent<Text>();
        titleText.Font = ResourceManager.Instance.GetFont("DefaultFont");
        titleText.Content = _isWin ? "You Win!" : "Game Over!";
        titleText.Color = _isWin ? Color.Gold : Color.Red;
        var titleSize = titleText.Font.MeasureString(titleText.Content);
        titleText.Origin = titleSize / 2;
        AddGameObject(titleObj);

        var timeObj = new GameObject
        {
            Position = new Vector2(centerX, centerY - 80),
            Scale = new Vector2(1.5f, 1.5f)
        };
        var timeText = timeObj.AddComponent<Text>();
        timeText.Font = ResourceManager.Instance.GetFont("DefaultFont");
        timeText.Content = $"Time: {_playTime}";
        timeText.Color = Color.White;
        var timeSize = timeText.Font.MeasureString(timeText.Content);
        timeText.Origin = timeSize / 2;
        AddGameObject(timeObj);

        float buttonWidth = 250f;
        float buttonHeight = 60f;
        float buttonY = centerY + 50;
        float buttonSpacing = 80;

        var playAgainButton = new TextButton(
            "Play Again",
            new Vector2(centerX - buttonWidth / 2, buttonY),
            new Vector2(buttonWidth, buttonHeight),
            OnPlayAgain,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(playAgainButton);

        var mainMenuButton = new TextButton(
            "Return to Menu",
            new Vector2(centerX - buttonWidth / 2, buttonY + buttonSpacing),
            new Vector2(buttonWidth, buttonHeight),
            OnMainMenu,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(mainMenuButton);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            OnMainMenu();
        }
    }
}
