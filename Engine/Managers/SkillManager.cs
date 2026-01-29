using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SlimeTogetherStrong.Engine.Managers;

public class SkillManager
{
    public static SkillManager Instance { get; private set; } = new SkillManager();

    private float _skillCooldownDuration = 5.0f; // 5 seconds cooldown
    private float _currentCooldown = 0f;
    private int _skillManaCost = 30; // Mana cost for skill
    private bool _isInPlacementMode = false;
    
    public bool IsOnCooldown => _currentCooldown > 0f;
    public float CooldownRemaining => _currentCooldown;
    public float CooldownDuration => _skillCooldownDuration;
    public float CooldownProgress => IsOnCooldown ? _currentCooldown / _skillCooldownDuration : 0f;
    public int SkillManaCost => _skillManaCost;
    public bool IsInPlacementMode => _isInPlacementMode;
    
    // Event to notify when entering/exiting placement mode
    public event System.Action<bool> OnPlacementModeChanged;
    // Event to notify when ally should be placed
    public event System.Action OnAllyPlaceRequested;

    private SkillManager() { }

    public void Update(GameTime gameTime)
    {
        // Update cooldown timer
        if (_currentCooldown > 0f)
        {
            _currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_currentCooldown < 0f)
            {
                _currentCooldown = 0f;
            }
        }

        // Check for Q key press to toggle placement mode
        if (InputManager.Instance.IsKeyPressed(Keys.Q))
        {
            TogglePlacementMode();
        }
    }

    private void TogglePlacementMode()
    {
        // Can only toggle if skill is not on cooldown
        if (IsOnCooldown)
        {
            System.Console.WriteLine($"Skill on cooldown: {_currentCooldown:F1}s remaining");
            return;
        }

        // Check if player has enough mana before entering placement mode
        var player = SlimeTogetherStrong.Game.GameScene.GetPlayer();
        if (player == null || player.ManaComponent == null)
        {
            System.Console.WriteLine("Player or mana system not found!");
            return;
        }

        if (!_isInPlacementMode)
        {
            // Check mana before entering placement mode
            if (player.ManaComponent.CurrentMana < _skillManaCost)
            {
                System.Console.WriteLine("Not enough mana!");
                return;
            }

            // Enter placement mode
            _isInPlacementMode = true;
            System.Console.WriteLine("Entered ally placement mode! Click to place or press Q to cancel.");
            OnPlacementModeChanged?.Invoke(true);
        }
        else
        {
            // Exit placement mode (cancel)
            _isInPlacementMode = false;
            System.Console.WriteLine("Exited ally placement mode.");
            OnPlacementModeChanged?.Invoke(false);
        }
    }

    public void OnAllyPlacedSuccessfully()
    {
        // Called by GameScene when ally is successfully placed
        if (!_isInPlacementMode)
            return;

        // Use mana and start cooldown
        var player = SlimeTogetherStrong.Game.GameScene.GetPlayer();
        if (player != null && player.ManaComponent != null)
        {
            if (player.ManaComponent.UseMana(_skillManaCost))
            {
                System.Console.WriteLine($"Ally placed! Used {_skillManaCost} mana.");
                _currentCooldown = _skillCooldownDuration;
                _isInPlacementMode = false;
                OnPlacementModeChanged?.Invoke(false);
            }
        }
    }

    public void NotifyAllyPlaceRequested()
    {
        // Notify that mouse click happened in placement mode
        if (_isInPlacementMode)
        {
            OnAllyPlaceRequested?.Invoke();
        }
    }

    public void ResetCooldown()
    {
        _currentCooldown = 0f;
    }
}
