using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

public class GameSceneUI
{
    private XPManager _xpManager;
    private XPBar _xpBar;
    private TimerUI _timerUI;
    private SkillCooldownUI _skillCooldownUI;
    private VerticalBarWithIcon _castleHealthBar;
    private VerticalBarWithIcon _playerManaBar;
    private Scene _scene;
    private Player _player;

    public GameSceneUI(Scene scene, XPManager xpManager, Player player)
    {
        _scene = scene;
        _xpManager = xpManager;
        _player = player;
    }

    public void Initialize()
    {
        CreateXPUI();
        CreateTimerUI();
        CreateSkillUI();
        CreateCastleHealthBar();
        CreatePlayerManaBar();
    }

    private void CreateXPUI()
    {
        // Create XP bar at the top of screen (flush to borders)
        _xpBar = new XPBar(
            new Vector2(0, 0),
            new Vector2(SceneManager.Instance.ScreenWidth, 15),
            _xpManager
        );
        _scene.AddGameObject(_xpBar);
    }

    private void CreateTimerUI()
    {
        // Create timer UI below XP bar (centered)
        float timerY = 20f; // Below XP bar (which is 15px tall)
        _timerUI = new TimerUI(
            new Vector2(SceneManager.Instance.ScreenWidth / 2, timerY)
        );
        _scene.AddGameObject(_timerUI);
    }

    private void CreateSkillUI()
    {
        // Create skill cooldown UI at bottom left of screen
        float iconSize = 100f;
        float padding = 10f;
        Vector2 position = new Vector2(
            padding,
            SceneManager.Instance.ScreenHeight - iconSize - padding
        );
        
        _skillCooldownUI = new SkillCooldownUI(position, new Vector2(iconSize, iconSize));
        _scene.AddGameObject(_skillCooldownUI);
    }

    private void CreateCastleHealthBar()
    {
        // Create castle health bar at bottom right (rightmost)
        float iconSize = 40f;
        float barWidth = 30f;
        float barHeight = 150f;
        float padding = 20f;
        
        // Position icon from bottom right (bar will auto-center under icon)
        Vector2 position = new Vector2(
            SceneManager.Instance.ScreenWidth - iconSize - padding,
            SceneManager.Instance.ScreenHeight - barHeight - iconSize - padding - 5f // -5f for gap
        );
        
        var healthIcon = ResourceManager.Instance.GetTexture("Health_Icon");
        
        _castleHealthBar = new VerticalBarWithIcon(
            position,
            new Vector2(iconSize, iconSize),
            new Vector2(barWidth, barHeight),
            healthIcon,
            Color.Red,           // Fill color
            Color.DarkRed,       // Background color
            Color.White,         // Outline color
            5,                   // Outline thickness
            () => GameScene.Castle?.GetComponent<SlimeTogetherStrong.Engine.Components.HealthComponent>()?.GetHealthPercentage() ?? 0f
        );
        _scene.AddGameObject(_castleHealthBar);
    }

    private void CreatePlayerManaBar()
    {
        // Create player mana bar at bottom right (left of HP bar)
        float iconSize = 40f;
        float barWidth = 30f;
        float barHeight = 150f;
        float padding = 20f;
        float spacing = 10f; // Space between mana and health bar icons
        
        // Position icon from bottom right, left of HP bar (bar will auto-center under icon)
        Vector2 position = new Vector2(
            SceneManager.Instance.ScreenWidth - (iconSize * 2) - padding - spacing,
            SceneManager.Instance.ScreenHeight - barHeight - iconSize - padding - 5f // -5f for gap
        );
        
        var manaIcon = ResourceManager.Instance.GetTexture("Mana_Icon");
        
        _playerManaBar = new VerticalBarWithIcon(
            position,
            new Vector2(iconSize, iconSize),
            new Vector2(barWidth, barHeight),
            manaIcon,
            Color.RoyalBlue,          // Fill color
            Color.MidnightBlue,      // Background color
            Color.White,         // Outline color
            5,                   // Outline thickness
            () => _player?.ManaComponent?.GetManaPercentage() ?? 0f
        );
        _scene.AddGameObject(_playerManaBar);
    }

    // Public accessors if needed
    public XPBar XPBar => _xpBar;
    public TimerUI TimerUI => _timerUI;
    public SkillCooldownUI SkillCooldownUI => _skillCooldownUI;
    public VerticalBarWithIcon CastleHealthBar => _castleHealthBar;
    public VerticalBarWithIcon PlayerManaBar => _playerManaBar;
}
