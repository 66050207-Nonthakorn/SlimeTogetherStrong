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
    private int _clearedWave;
    private int _newLevel;

    private bool _isInCountdown = true;
    private float _countdownTimer = 2.0f;
    private const float COUNTDOWN_DURATION = 2.0f;
    private bool _upgradeUICreated = false;

    private GameObject _waveClearTitle;
    private GameObject _countdownText;
    private GameObject _levelUpSubtitle;

    // UI elements for upgrade phase (created after countdown)
    private List<GameObject> _upgradeCards = new List<GameObject>();
    private GameObject _chooseTitleObj;
    private GameObject _chooseSubtitleObj;

    public LevelUpOverlayScene(List<UpgradeOption> upgradeOptions, Action<int> onUpgradeSelected, int clearedWave, int newLevel)
    {
        _upgradeOptions = upgradeOptions;
        _onUpgradeSelected = onUpgradeSelected;
        _clearedWave = clearedWave;
        _newLevel = newLevel;
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

        _waveClearTitle = new GameObject
        {
            Position = new Vector2(centerX, centerY - 80),
            Scale = new Vector2(2.5f, 2.5f)
        };
        var waveClearText = _waveClearTitle.AddComponent<Text>();
        waveClearText.Font = ResourceManager.Instance.GetFont("DefaultFont");
        waveClearText.Content = $"Wave {_clearedWave} Clear!";
        waveClearText.Color = Color.LimeGreen;
        var waveClearSize = waveClearText.Font.MeasureString(waveClearText.Content);
        waveClearText.Origin = waveClearSize / 2;
        AddGameObject(_waveClearTitle);

        _levelUpSubtitle = new GameObject
        {
            Position = new Vector2(centerX, centerY),
            Scale = new Vector2(1.8f, 1.8f)
        };
        var levelUpText = _levelUpSubtitle.AddComponent<Text>();
        levelUpText.Font = ResourceManager.Instance.GetFont("DefaultFont");
        levelUpText.Content = $"Level Up! Lv.{_newLevel}";
        levelUpText.Color = Color.Yellow;
        var levelUpSize = levelUpText.Font.MeasureString(levelUpText.Content);
        levelUpText.Origin = levelUpSize / 2;
        AddGameObject(_levelUpSubtitle);

        _countdownText = new GameObject
        {
            Position = new Vector2(centerX, centerY + 80),
            Scale = new Vector2(1.5f, 1.5f)
        };
        var countdownTextComp = _countdownText.AddComponent<Text>();
        countdownTextComp.Font = ResourceManager.Instance.GetFont("DefaultFont");
        countdownTextComp.Content = $"{(int)Math.Ceiling(_countdownTimer)}";
        countdownTextComp.Color = Color.White;
        var countdownSize = countdownTextComp.Font.MeasureString(countdownTextComp.Content);
        countdownTextComp.Origin = countdownSize / 2;
        AddGameObject(_countdownText);
    }

    private void CreateUpgradeUI()
    {
        if (_upgradeUICreated) return;
        _upgradeUICreated = true;

        int screenWidth = SceneManager.Instance.ScreenWidth;
        int screenHeight = SceneManager.Instance.ScreenHeight;

        float cardWidth = 220f;
        float cardHeight = 280f;
        float cardSpacing = 30f;
        float totalWidth = (cardWidth * Math.Min(3, _upgradeOptions.Count)) + (cardSpacing * (Math.Min(3, _upgradeOptions.Count) - 1));
        float startX = (screenWidth - totalWidth) / 2;
        float cardY = (screenHeight - cardHeight) / 2;

        float titleY = cardY - 130;
        float subtitleY = cardY - 70;

        _chooseTitleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, titleY),
            Scale = new Vector2(2.5f, 2.5f)
        };
        var title = _chooseTitleObj.AddComponent<Text>();
        title.Font = ResourceManager.Instance.GetFont("DefaultFont");
        title.Content = $"Level {_newLevel}!";
        title.Color = Color.Yellow;
        var titleSize = title.Font.MeasureString(title.Content);
        title.Origin = titleSize / 2;
        AddGameObject(_chooseTitleObj);

        _chooseSubtitleObj = new GameObject
        {
            Position = new Vector2(screenWidth / 2, subtitleY),
            Scale = new Vector2(1.2f, 1.2f)
        };
        var subtitle = _chooseSubtitleObj.AddComponent<Text>();
        subtitle.Font = ResourceManager.Instance.GetFont("DefaultFont");
        subtitle.Content = "Choose 1 upgrade";
        subtitle.Color = Color.White;
        var subtitleSize = subtitle.Font.MeasureString(subtitle.Content);
        subtitle.Origin = subtitleSize / 2;
        AddGameObject(_chooseSubtitleObj);

        for (int i = 0; i < Math.Min(3, _upgradeOptions.Count); i++)
        {
            var option = _upgradeOptions[i];
            int cardIndex = i;

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
            _upgradeCards.Add(card);
        }
    }

    private void SetCountdownUIVisible(bool visible)
    {
        if (_waveClearTitle != null) _waveClearTitle.Active = visible;
        if (_countdownText != null) _countdownText.Active = visible;
        if (_levelUpSubtitle != null) _levelUpSubtitle.Active = visible;
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            SceneManager.Instance.PushOverlay(new PauseScene());
            return;
        }

        if (_isInCountdown)
        {
            _countdownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            var countdownTextComp = _countdownText?.GetComponent<Text>();
            if (countdownTextComp != null)
            {
                int secondsLeft = (int)Math.Ceiling(_countdownTimer);
                countdownTextComp.Content = secondsLeft > 0 ? $"{secondsLeft}" : "";
                var size = countdownTextComp.Font.MeasureString(countdownTextComp.Content);
                countdownTextComp.Origin = size / 2;
            }

            if (_countdownTimer <= 0)
            {
                _isInCountdown = false;
                SetCountdownUIVisible(false);
                CreateUpgradeUI(); // Create upgrade UI only now!
            }
            return;
        }

        base.Update(gameTime);
    }

    private void OnCardSelected(int cardIndex)
    {
        if (_isInCountdown) return;
        _onUpgradeSelected?.Invoke(cardIndex);
        SceneManager.Instance.PopOverlay();
    }
}

public class UpgradeOption
{
    public Texture2D EntityTexture { get; set; }
    public Texture2D StatsIconTexture { get; set; }
    public string EntityName { get; set; }
    public string StatsName { get; set; }
    public int StatsIncrease { get; set; }
}
