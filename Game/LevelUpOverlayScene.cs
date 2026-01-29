using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public class LevelUpOverlayScene : Scene
{
    private Action<int> _onUpgradeSelected;
    private List<UpgradeOption> _upgradeOptions;
    
    public LevelUpOverlayScene(List<UpgradeOption> upgradeOptions, Action<int> onUpgradeSelected)
    {
        _upgradeOptions = upgradeOptions;
        _onUpgradeSelected = onUpgradeSelected;
    }

    public override void Load()
    {
        base.Load();
        
        int screenWidth = SceneManager.Instance.ScreenWidth;
        int screenHeight = SceneManager.Instance.ScreenHeight;
        
        // Create dim overlay background
        var overlayBg = new GameObject
        {
            Position = Vector2.Zero
        };
        var overlayRenderer = overlayBg.AddComponent<RoundedRectangleRenderer>();
        overlayRenderer.Size = new Vector2(screenWidth, screenHeight);
        overlayRenderer.FillColor = new Color(0, 0, 0, 180); // Black with opacity
        overlayRenderer.OutlineThickness = 0;
        AddGameObject(overlayBg);

        // Create upgrade cards (centered)
        float cardWidth = 220f;
        float cardHeight = 280f;
        float cardSpacing = 30f;
        float totalWidth = (cardWidth * Math.Min(3, _upgradeOptions.Count)) + (cardSpacing * (Math.Min(3, _upgradeOptions.Count) - 1));
        float startX = (screenWidth - totalWidth) / 2;
        float cardY = (screenHeight - cardHeight) / 2;

        // Position titles relative to cards
        float titleY = cardY - 130;
        float subtitleY = cardY - 70;

        // "Level Up" title
        var titleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, titleY),
            Scale = new Vector2(2.5f, 2.5f)
        };
        var title = titleObj.AddComponent<Text>();
        title.Font = ResourceManager.Instance.GetFont("DefaultFont");
        title.Content = "Level Up!";
        title.Color = Color.Yellow;
        var titleSize = title.Font.MeasureString(title.Content);
        title.Origin = titleSize / 2;
        AddGameObject(titleObj);

        // "Choose 1 upgrade" subtitle
        var subtitleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, subtitleY),
            Scale = new Vector2(1.2f, 1.2f)
        };
        var subtitle = subtitleObj.AddComponent<Text>();
        subtitle.Font = ResourceManager.Instance.GetFont("DefaultFont");
        subtitle.Content = "Choose 1 upgrade";
        subtitle.Color = Color.White;
        var subtitleSize = subtitle.Font.MeasureString(subtitle.Content);
        subtitle.Origin = subtitleSize / 2;
        AddGameObject(subtitleObj);

        for (int i = 0; i < Math.Min(3, _upgradeOptions.Count); i++)
        {
            var option = _upgradeOptions[i];
            int cardIndex = i; // Capture for lambda
            
            var card = new UpgradeCard(
                new Vector2(startX + (cardWidth + cardSpacing) * i, cardY),
                new Vector2(cardWidth, cardHeight),
                option.EntityTexture,
                option.StatsIconTexture,
                option.EntityName,
                option.StatsName,
                option.StatsIncrease,
                () => OnCardSelected(cardIndex),
                ResourceManager.Instance.GetFont("DefaultFont")
            );
            
            AddGameObject(card);
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Check for ESC key to open pause menu
        if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            SceneManager.Instance.PushOverlay(new PauseScene());
            return;
        }

        base.Update(gameTime);
    }

    private void OnCardSelected(int cardIndex)
    {
        // Invoke callback with selected index
        _onUpgradeSelected?.Invoke(cardIndex);
        
        // Close this overlay
        SceneManager.Instance.PopOverlay();
    }
}

// Data class for upgrade options
public class UpgradeOption
{
    public Texture2D EntityTexture { get; set; }
    public Texture2D StatsIconTexture { get; set; }
    public string EntityName { get; set; }
    public string StatsName { get; set; }
    public int StatsIncrease { get; set; }
}
