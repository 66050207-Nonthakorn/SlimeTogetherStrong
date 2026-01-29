using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public class SettingsScene : Scene
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
        // Create a semi-transparent overlay
        AddGameObject(overlayBg);

        // Panel dimensions
        float panelWidth = 600;
        float panelHeight = 500;
        float panelX = (screenWidth - panelWidth) / 2;
        float panelY = (screenHeight - panelHeight) / 2;

        // Settings panel background (rounded rectangle)
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

        // Settings title
        var titleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, panelY + 60)
        };
        var title = titleObj.AddComponent<Text>();
        title.Font = ResourceManager.Instance.GetFont("DefaultFont");
        title.Content = "Settings";
        title.Color = Color.White;
        titleObj.Scale = new Vector2(2, 2);
        var titleSize = title.MeasureText();
        title.Origin = titleSize / 2;
        AddGameObject(titleObj);

        // Content area starting position
        float contentX = panelX + 80;
        float contentStartY = panelY + 150;
        float rowSpacing = 100;

        // BGM Label
        var bgmLabelObj = new GameObject
        {
            Position = new Vector2(contentX, contentStartY)
        };
        var bgmLabel = bgmLabelObj.AddComponent<Text>();
        bgmLabel.Font = ResourceManager.Instance.GetFont("DefaultFont");
        bgmLabel.Content = "BGM Volume:";
        bgmLabel.Color = Color.White;
        AddGameObject(bgmLabelObj);

        // BGM Slider
        var bgmSliderObj = new GameObject
        {
            Position = new Vector2(contentX + 200, contentStartY - 10)
        };
        var bgmSlider = bgmSliderObj.AddComponent<Slider>();
        bgmSlider.Size = new Vector2(250, 20);
        bgmSlider.MinValue = 0f;
        bgmSlider.MaxValue = 1f;
        bgmSlider.CurrentValue = AudioManager.Instance.BGMVolume;
        bgmSlider.BaseColor = new Color(80, 80, 80);
        bgmSlider.TrackColor = new Color(100, 200, 100);
        bgmSlider.OnValueChanged = (value) =>
        {
            AudioManager.Instance.BGMVolume = value;
        };
        AddGameObject(bgmSliderObj);

        // Sound Effects Label
        var sfxLabelObj = new GameObject
        {
            Position = new Vector2(contentX, contentStartY + rowSpacing)
        };
        var sfxLabel = sfxLabelObj.AddComponent<Text>();
        sfxLabel.Font = ResourceManager.Instance.GetFont("DefaultFont");
        sfxLabel.Content = "SFX Volume:";
        sfxLabel.Color = Color.White;
        AddGameObject(sfxLabelObj);

        // Sound Effects Slider
        var sfxSliderObj = new GameObject
        {
            Position = new Vector2(contentX + 200, contentStartY + rowSpacing - 10)
        };
        var sfxSlider = sfxSliderObj.AddComponent<Slider>();
        sfxSlider.Size = new Vector2(250, 20);
        sfxSlider.MinValue = 0f;
        sfxSlider.MaxValue = 1f;
        sfxSlider.CurrentValue = AudioManager.Instance.SFXVolume;
        sfxSlider.BaseColor = new Color(80, 80, 80);
        sfxSlider.TrackColor = new Color(100, 200, 100);
        sfxSlider.OnValueChanged = (value) =>
        {
            AudioManager.Instance.SFXVolume = value;
        };
        AddGameObject(sfxSliderObj);

        // Back Button
        float buttonWidth = 200;
        float buttonHeight = 60;
        float buttonX = (screenWidth - buttonWidth) / 2;
        float buttonY = panelY + panelHeight - 100;

        var backButton = new TextButton(
            "Back",
            new Vector2(buttonX, buttonY),
            new Vector2(buttonWidth, buttonHeight),
            OnBackClick,
            ResourceManager.Instance.GetFont("DefaultFont")
        );
        AddGameObject(backButton);

        base.Load();
    }

    private void OnBackClick()
    {
        // Close the settings overlay
        SceneManager.Instance.PopOverlay();
    }
}
